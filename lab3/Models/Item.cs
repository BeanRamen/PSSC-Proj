using System;
using System.Collections.Generic;

namespace lab3.Models
{
    public static class Item
    {
        public interface IItem { }

        public record UnvalidatedItem : IItem
        {
            public UnvalidatedItem(IReadOnlyCollection<UnvalidatedCartPrice> priceList)
            {
                PriceList = priceList;
            }

            public IReadOnlyCollection<UnvalidatedCartPrice> PriceList { get; }
        }

        public record InvalidItem : IItem
        {
            internal InvalidItem(IReadOnlyCollection<UnvalidatedCartPrice> priceList, IEnumerable<string> reasons)
            {
                PriceList = priceList;
                Reasons = reasons;
            }

            public IReadOnlyCollection<UnvalidatedCartPrice> PriceList { get; }
            public IEnumerable<string> Reasons { get; }
        }

        public record ValidatedItem : IItem
        {
            internal ValidatedItem(IReadOnlyCollection<ValidatedCartPrice> pricesList)
            {
                PriceList = pricesList;
            }

            public IReadOnlyCollection<ValidatedCartPrice> PriceList { get; }
        }

        public record CalculatedItem : IItem
        {
            internal CalculatedItem(IReadOnlyCollection<CalculatedCartPrice> pricesList)
            {
                PriceList = pricesList;
            }

            public IReadOnlyCollection<CalculatedCartPrice> PriceList { get; }
        }

        public record PublishedItem : IItem
        {
            internal PublishedItem(IReadOnlyCollection<CalculatedCartPrice> pricesList, string csv, DateTime publishedDate)
            {
                PriceList = pricesList;
                PublishedDate = publishedDate;
                Csv = csv;
            }

            public IReadOnlyCollection<CalculatedCartPrice> PriceList { get; }
            public DateTime PublishedDate { get; }
            public string Csv { get; }
        }
    }
}