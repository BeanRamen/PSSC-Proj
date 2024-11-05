using lab3.Models;
using System.Collections.Generic;
using System.Linq;

namespace lab3.Operations
{
    internal sealed class CalculateItemOperation : ItemOperation
    {
        internal CalculateItemOperation()
        {
        }
        protected override Item.IItem OnValid(Item.ValidatedItem validItemPrices)
        {
            IEnumerable<CalculatedCartPrice> calculatedPrice = validItemPrices.PriceList
                .Select(validItem =>
                    new CalculatedCartPrice(
                        validItem.CartRegistrationNumber,
                        validItem.ItemPrice,
                        validItem.TVA,
                        CalculateFinalGrade(validItem)));
            return new Item.CalculatedItem(calculatedPrice.ToList().AsReadOnly());
        }

        private static Price? CalculateFinalGrade(ValidatedCartPrice validPrice)
        {
            return validPrice.ItemPrice is not null
                   && validPrice.ItemPrice.Value >= 5
                   && validPrice.TVA is not null
                   && validPrice.TVA.Value >= 5
                ? validPrice.ItemPrice + validPrice.TVA
                : null;
        }
    }
}