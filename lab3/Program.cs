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
      Console.WriteLine("Catalog Preturi:");
      Console.WriteLine("============================");
      
      Console.WriteLine(consoleMessage);
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