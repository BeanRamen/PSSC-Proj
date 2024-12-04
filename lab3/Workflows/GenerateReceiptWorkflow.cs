using System.Text;
using lab3.Models;
using lab3.Repos;

namespace lab3.Workflows
{
    public class GenerateReceiptWorkflow
    {
        private readonly IPricesRepository _pricesRepository;

        public GenerateReceiptWorkflow(IPricesRepository pricesRepository)
        {
            _pricesRepository = pricesRepository;
        }

        public async Task<string> ExecuteAsync(CartRegistrationNumber cartRegistrationNumber)
        {
            var prices = await _pricesRepository.GetExistingPricesAsync();

            var cartPrices = prices.Where(p => p.CartRegistrationNumber.Value == cartRegistrationNumber.Value).ToList();

            if (!cartPrices.Any())
            {
                return $"No items found for cart: {cartRegistrationNumber.Value}";
            }

            return GenerateReceipt(cartPrices);
        }

        private static string GenerateReceipt(IEnumerable<CalculatedCartPrice> cartPrices)
        {
            StringBuilder receipt = new();
            receipt.AppendLine("Chitanta pentru cos:");
            receipt.AppendLine("Numar Inregistrare, Pret Initial, Reducere (TVA), Pret Final");

            decimal total = 0;

            foreach (var price in cartPrices)
            {
                receipt.AppendLine($"{price.CartRegistrationNumber.Value}, {price.ItemPrice}, {price.TVA}, {price.FinalPrice}");
                if (price.FinalPrice != null)
                {
                    total += price.FinalPrice.Value;
                }
            }

            receipt.AppendLine($"Total: {total:0.00}");
            return receipt.ToString();
        }
    }
}