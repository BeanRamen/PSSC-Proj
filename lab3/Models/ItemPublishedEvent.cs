using System;
using System.Collections.Generic;
using static lab3.Models.Item;

namespace lab3.Models
{
    public static class ItemPublishedEvent
    {
        public interface IItemPublishedEvent { }

        public record ItemPublishSucceededEvent : IItemPublishedEvent
        {
            public string Csv { get; }
            public DateTime PublishedDate { get; }

            internal ItemPublishSucceededEvent(string csv, DateTime publishedDate)
            {
                Csv = csv;
                PublishedDate = publishedDate;
            }
        }

        public record ItemPublishFailedEvent : IItemPublishedEvent
        {
            public IEnumerable<string> Reasons { get; }

            internal ItemPublishFailedEvent(string reason)
            {
                Reasons = [reason];
            }

            internal ItemPublishFailedEvent(IEnumerable<string> reasons)
            {
                Reasons = reasons;
            }
        }

        public static IItemPublishedEvent ToEvent(this IItem item) =>
            item switch
            {
                UnvalidatedItem _ => new ItemPublishFailedEvent("Unexpected unvalidated state"),
                ValidatedItem validatedPrices => new ItemPublishFailedEvent("Unexpected validated state"),
                CalculatedItem calculatedPrices => new ItemPublishFailedEvent("Unexpected calculated state"),
                InvalidItem invalidPrices => new ItemPublishFailedEvent(invalidPrices.Reasons),
                PublishedItem publishedPrices => new ItemPublishSucceededEvent(publishedPrices.Csv, publishedPrices.PublishedDate),
                _ => throw new NotImplementedException()
            };
    }
}