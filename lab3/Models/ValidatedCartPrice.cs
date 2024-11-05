namespace lab3.Models
{
    public record ValidatedCartPrice(CartRegistrationNumber CartRegistrationNumber, Price? ItemPrice, Price? TVA);
}