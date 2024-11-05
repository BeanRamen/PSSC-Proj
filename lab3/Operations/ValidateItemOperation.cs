using System;
using System.Collections.Generic;
using System.Linq;
using lab3.Models;

namespace lab3.Operations
{
  internal sealed class ValidateItemOperation : ItemOperation
  {
    private readonly Func<CartRegistrationNumber, bool> checkCartExists;

    internal ValidateItemOperation(Func<CartRegistrationNumber, bool> checkCartExists)
    {
      this.checkCartExists = checkCartExists;
    }

    protected override Item.IItem OnUnvalidated(Item.UnvalidatedItem unvalidatedItem)
    {
      (List<ValidatedCartPrice> validatedPrices, IEnumerable<string> validationErrors) = ValidateListOfGrades(unvalidatedItem);

      if (validationErrors.Any())
      {
        return new Item.InvalidItem(unvalidatedItem.PriceList, validationErrors);
      }
      else
      {
        return new Item.ValidatedItem(validatedPrices);
      }
    }

    private (List<ValidatedCartPrice>, IEnumerable<string>) ValidateListOfGrades(Item.UnvalidatedItem itemPrices)
    {
      List<string> validationErrors = [];
      List<ValidatedCartPrice> validatedPrices = [];
      foreach (UnvalidatedCartPrice unvalidatedPrice in itemPrices.PriceList)
      {
        ValidatedCartPrice? validPrice = ValidatePrice(unvalidatedPrice, validationErrors);
        if (validPrice is not null)
        {
          validatedPrices.Add(validPrice);
        }
      }

      return (validatedPrices, validationErrors);
    }

    private ValidatedCartPrice? ValidatePrice(UnvalidatedCartPrice unvalidatedPrice, List<string> validationErrors)
    {
      List<string> currentValidationErrors = [];
      Price? itemPrice = ValidateAndParseExamGrade(unvalidatedPrice, currentValidationErrors);
      Price? TVA = ValidateAndParseActivityGrade(unvalidatedPrice, currentValidationErrors);
      CartRegistrationNumber? cartRegistrationNumber = ValidateAndParseRegistrationNumber(unvalidatedPrice, currentValidationErrors);

      ValidatedCartPrice? validPrice = null;
      if (!currentValidationErrors.Any())
      {
        validPrice = new(cartRegistrationNumber!, itemPrice, TVA!);
      }
      else
      {
        validationErrors.AddRange(currentValidationErrors);
      }
      return validPrice;
    }

    private static Price? ValidateAndParseExamGrade(UnvalidatedCartPrice unvalidatedPrice, List<string> validationErrors)
    {
      if (!Price.TryParsePrice(unvalidatedPrice.ItemPrice, out Price? itemPrice))
      {
        validationErrors.Add($"Invalid item price ({unvalidatedPrice.CartRegistrationNumber}, {unvalidatedPrice.ItemPrice})");
      }

      return itemPrice;
    }

    private static Price? ValidateAndParseActivityGrade(UnvalidatedCartPrice unvalidatedPrice, List<string> validationErrors)
    {
      Price? tva;
      if (!Price.TryParsePrice(unvalidatedPrice.TVA, out tva))
      {
        validationErrors.Add($"Invalid TVA ({unvalidatedPrice.CartRegistrationNumber}, {unvalidatedPrice.TVA})");
      }

      return tva;
    }

    private CartRegistrationNumber? ValidateAndParseRegistrationNumber(UnvalidatedCartPrice unvalidatedPrice, List<string> validationErrors)
    {
      CartRegistrationNumber? cartRegistrationNumber;
      if (!CartRegistrationNumber.TryParse(unvalidatedPrice.CartRegistrationNumber, out cartRegistrationNumber))
      {
        validationErrors.Add($"Invalid cart registration number ({unvalidatedPrice.CartRegistrationNumber})");
      }
      else if (!checkCartExists(cartRegistrationNumber!))
      {
        validationErrors.Add($"Cart not found ({unvalidatedPrice.CartRegistrationNumber})");
      }

      return cartRegistrationNumber;
    }
  }
}