using lab3.Models;
using lab3.Workflows;
using lab3.Repos;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using static lab3.Models.ItemPublishedEvent;
using PricesContext = lab3.Repos.PricesContext;

namespace lab3
{
  internal class Program
  {
    private static string ConnectionString = "Server=localhost\\SQLEXPRESS;Database=Cart;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=true";
    
    private static async Task Main(string[] args)
    {
      using ILoggerFactory loggerFactory = ConfigureLoggerFactory();
      ILogger<PublishItemWorkflow> logger = loggerFactory.CreateLogger<PublishItemWorkflow>();
      DbContextOptionsBuilder<PricesContext> dbContextBuilder = new DbContextOptionsBuilder<PricesContext>()
        .UseSqlServer(ConnectionString)
        .UseLoggerFactory(loggerFactory);
      PricesContext pricesContext = new(dbContextBuilder.Options);
      CartsRepository cartsRepository = new(pricesContext);
      PricesRepository pricesRepository = new(pricesContext);
      
      //get user input
      UnvalidatedCartPrice[] listOfPrices = ReadListOfPrices().ToArray();

      //execute domain workflow
      PublishItemCommand command = new(listOfPrices);
      PublishItemWorkflow workflow = new(cartsRepository, pricesRepository, logger);
      IItemPublishedEvent result = await workflow.ExecuteAsync(command);

      string consoleMessage = result switch
      {
        ItemPublishSucceededEvent @event => @event.Csv,
        ItemPublishFailedEvent @event => $"Publish failed: \r\n{string.Join("\r\n", @event.Reasons)}",
        _ => throw new NotImplementedException()
      };

      Console.WriteLine();
      Console.WriteLine("============================");
      Console.WriteLine("    Newly added items:");
      Console.WriteLine("============================");
      
      Console.WriteLine(consoleMessage);
      
      string? proceedWithCheckout = ReadValue("Proceed With Checkout? (y/n): ");
      string? awb = null;
      if (proceedWithCheckout != null && proceedWithCheckout.Equals("y", StringComparison.OrdinalIgnoreCase))
      {
        string? cartRegistrationNumber = ReadValue("Enter the cart registration number for modifications and receipt processing: ");

        if (!string.IsNullOrEmpty(cartRegistrationNumber))
        {
          // Afisare produse din cos
          GenerateReceiptWorkflow generateReceiptWorkflow = new(pricesRepository);
          string receipt = await generateReceiptWorkflow.ExecuteAsync(new CartRegistrationNumber(cartRegistrationNumber));
          Console.WriteLine(receipt);

          // Modificare cos
          ModifyCartWorkflow modifyCartWorkflow = new(pricesRepository);
          while (true)
          {
            string cartDetails = await modifyCartWorkflow.ExecuteAsync(cartRegistrationNumber);
            Console.WriteLine(cartDetails);

            string? continueModifying = ReadValue("Would you like to make more changes? (y/n): ");
            if (continueModifying == null || continueModifying.Equals("n", StringComparison.OrdinalIgnoreCase))
            {
              break;
            }
          }

          // Generare AWB și adresa
          string? generateAWB = ReadValue("Generate AWB and delivery address? (y/n): ");
          if (generateAWB != null && generateAWB.Equals("y", StringComparison.OrdinalIgnoreCase))
          {
            GenerateAWBWorkflow generateAWBWorkflow = new();
            var (address, generatedAWB) = await generateAWBWorkflow.ExecuteAsync();
            awb = generatedAWB;
            Console.WriteLine($"Delivery Address: {address}");
            Console.WriteLine($"AWB: {awb}");
          }

          // Generare chitanta finala
          string finalReceipt = await generateReceiptWorkflow.ExecuteAsync(new CartRegistrationNumber(cartRegistrationNumber));
          Console.WriteLine(finalReceipt);
          
          if (!string.IsNullOrEmpty(awb))
          {
            Console.WriteLine($"AWB: {awb}");
          }
          else
          {
            Console.WriteLine("AWB not generated.");
          }
        }
      }
      else
      {
        Console.WriteLine("Keep shopping");
      }
      
    }

    private static ILoggerFactory ConfigureLoggerFactory()
    {
      return LoggerFactory.Create(builder =>
        builder.AddSimpleConsole(options =>
          {
            options.IncludeScopes = true;
            options.SingleLine = true;
            options.TimestampFormat = "hh:mm:ss ";
          })
          .AddProvider(new Microsoft.Extensions.Logging.Debug.DebugLoggerProvider()));
    }

    private static List<UnvalidatedCartPrice> ReadListOfPrices()
    {
      List<UnvalidatedCartPrice> listOfPrices = [];
      do
      {
        //read registration number and price and create a list of prices
        string? registrationNumber = ReadValue("Registration Number: ");
        if (string.IsNullOrEmpty(registrationNumber))
        {
          break;
        }

        string? itemPrice = ReadValue("Item Price: ");
        if (string.IsNullOrEmpty(itemPrice))
        {
          break;
        }

        string? TVA = ReadValue("Reducere: ");
        if (string.IsNullOrEmpty(TVA))
        {
          break;
        }

        listOfPrices.Add(new(registrationNumber, itemPrice, TVA));
      } while (true);
      return listOfPrices;
    }

    private static string? ReadValue(string prompt)
    {
      Console.Write(prompt);
      return Console.ReadLine();
    }
  }
}