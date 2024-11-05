using lab3.Exceptions;
using System;

namespace lab3.Models
{
    public record Price
    {

        public decimal Value { get; }

        private Price()
        {
            Value = 0;
        }

        public Price(decimal value)
        {
            if (IsValid(value))
            {
                Value = value;
            }
            else
            {
                throw new InvalidPriceException($"{value:0.##} is an invalid price value.");
            }
        }

        public static Price operator +(Price a, Price b) => new(a.Value -(a.Value *b.Value/100));


        public Price Round()
        {
            decimal roundedValue = Math.Round(Value);
            return new Price(roundedValue);
        }

        public override string ToString() => $"{Value:0.##}";

        public static bool TryParsePrice(string? priceString, out Price? price)
        {
            bool isValid = false;
            price = null;
            if (decimal.TryParse(priceString, out decimal numericPrice))
            {
                if (IsValid(numericPrice))
                {
                    isValid = true;
                    price = new(numericPrice);
                }
            }

            return isValid;
        }

        private static bool IsValid(decimal numericGrade) => numericGrade > 0 && numericGrade <= 10000;
    }
}