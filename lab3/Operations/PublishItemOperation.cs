using lab3.Models;
using System;
using System.Text;

namespace lab3.Operations
{
    internal sealed class PublishItemOperation : ItemOperation
    {
        protected override Item.IItem OnCalculated(Item.CalculatedItem calculatedItem)
        {
            StringBuilder csv = new();
            calculatedItem.PriceList.Aggregate(csv, (export, price) =>
                export.AppendLine(GenerateCsvLine(price)));

            Item.PublishedItem publishedItemPrices = new(calculatedItem.PriceList, csv.ToString(), DateTime.Now);
            return publishedItemPrices;
        }

        private static string GenerateCsvLine(CalculatedCartPrice price) =>
            $"{price.CartRegistrationNumber.Value}, {price.ItemPrice}, {price.TVA}, {price.FinalPrice}";
    }
}