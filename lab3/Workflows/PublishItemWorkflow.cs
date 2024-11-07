﻿using lab3.Models;
using lab3.Operations;
using lab3.Repos;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static lab3.Models.Item;
using static lab3.Models.ItemPublishedEvent;

namespace lab3.Workflows
{
  public class PublishItemWorkflow
  {
    private readonly ICartsRepository cartsRepository;
    private readonly IPricesRepository pricesRepository;
    private readonly ILogger<PublishItemWorkflow> logger;

    public PublishItemWorkflow(ICartsRepository cartsRepository, IPricesRepository pricesRepository, ILogger<PublishItemWorkflow> logger)
    {
      this.cartsRepository = cartsRepository;
      this.pricesRepository = pricesRepository;
      this.logger = logger;
    }

    public async Task<IItemPublishedEvent> ExecuteAsync(PublishItemCommand command)
    {
      try
      {
        //load state from database
        IEnumerable<string> cartsToCheck = command.InputItemPrices.Select(price => price.CartRegistrationNumber);
        List<CartRegistrationNumber> existingCarts = await cartsRepository.GetExistingCartsAsync(cartsToCheck);
        List<CalculatedCartPrice> existingPrices = await pricesRepository.GetExistingPricesAsync();

        //execute pure business logic
        IItem item = ExecuteBusinessLogic(command, existingCarts, existingPrices);

        //save new state to database
        if (item is PublishedItem publishedItem)
        {
          await pricesRepository.SavePricesAsync(publishedItem);
        }

        //evaluate the state of the entity and generate the appropriate event
        return item.ToEvent();
      }
      catch (Exception ex)
      {
        logger.LogError(ex, "An error occurred while publishing prices");
        return new ItemPublishFailedEvent("Unexpected error");
      }
    }

    private static IItem ExecuteBusinessLogic(
      PublishItemCommand command,
      List<CartRegistrationNumber> existingCarts,
      List<CalculatedCartPrice> existingPrices)
    {
      Func<CartRegistrationNumber, bool> checkCartExists = cart => existingCarts.Any(s => s.Equals(cart));
      UnvalidatedItem unvalidatedPrices = new(command.InputItemPrices);

      IItem item = new ValidateItemOperation(checkCartExists).Transform(unvalidatedPrices);
      item = new CalculateItemOperation().Transform(item, existingPrices);
      item = new PublishItemOperation().Transform(item);
      return item;
    }
  }
}