namespace lab3.Models
{
    public record CalculatedCartPrice(CartRegistrationNumber CartRegistrationNumber, Price? ItemPrice, Price? TVA, Price? FinalPrice);
}