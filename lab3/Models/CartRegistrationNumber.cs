using System.Text.RegularExpressions;
using lab3.Exceptions;

namespace lab3.Models
{
    public record CartRegistrationNumber
    {
        private static readonly Regex ValidPattern = new(".*");

        public string Value { get; }

        private CartRegistrationNumber(string value)
        {
            if (IsValid(value))
            {
                Value = value;
            }
            else
            {
                throw new InvalidCartRegistrationNumberException("");
            }
        }

        private static bool IsValid(string stringValue) => ValidPattern.IsMatch(stringValue);

        public override string ToString()
        {
            return Value;
        }

        public static bool TryParse(string stringValue, out CartRegistrationNumber? registrationNumber)
        {
            bool isValid = false;
            registrationNumber = null;

            if (IsValid(stringValue))
            {
                isValid = true;
                registrationNumber = new(stringValue);
            }

            return isValid;
        }
    }
}