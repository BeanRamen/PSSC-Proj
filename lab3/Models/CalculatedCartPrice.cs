namespace lab3.Models
{
    public record CalculatedCartPrice(CartRegistrationNumber CartRegistrationNumber, Price? ItemPrice, Price? TVA, Price? FinalPrice)
    {
        public int PriceId { get; init; }
        public bool IsUpdated { get; init; }
    }
}