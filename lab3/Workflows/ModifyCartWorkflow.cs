using System.Text;
using lab3.Models;
using lab3.Repos;

namespace lab3.Workflows
{
    public class ModifyCartWorkflow
    {
        private readonly IPricesRepository _pricesRepository;

        public ModifyCartWorkflow(IPricesRepository pricesRepository)
        {
            _pricesRepository = pricesRepository;
        }

        public async Task<string> ExecuteAsync(string cartRegistrationNumber)
        {
            var prices = await _pricesRepository.GetExistingPricesAsync();
            var cartPrices = prices.Where(p => p.CartRegistrationNumber.Value == cartRegistrationNumber).ToList();

            if (!cartPrices.Any())
                return "No products found for the selected cart.";

            StringBuilder cartDetails = new();
            cartDetails.AppendLine("Nr. | Product | Initial Price | VAT (%) | Final Price");
            cartDetails.AppendLine("---------------------------------------------------");

            for (int i = 0; i < cartPrices.Count; i++)
            {
                var price = cartPrices[i];
                cartDetails.AppendLine($"{i + 1}. {price.CartRegistrationNumber} | {price.ItemPrice?.Value:0.00} | {price.TVA?.Value:0.00} | {price.FinalPrice?.Value:0.00}");
            }

            string? option = ReadValue("Select product number to modify or delete, or 'c' to cancel: ");
            if (option?.ToLower() == "c")
                return "No changes made.";

            if (int.TryParse(option, out int productIndex) && productIndex >= 1 && productIndex <= cartPrices.Count)
            {
                var selectedProduct = cartPrices[productIndex - 1];
                string? action = ReadValue("[1] Modify [2] Delete: ");

                if (action == "1")
                {
                    string? newPrice = ReadValue("Enter new price: ");
                    string? newVAT = ReadValue("Enter new VAT (as percentage): ");

                    var updatedItemPrice = new Price(decimal.Parse(newPrice ?? selectedProduct.ItemPrice?.Value.ToString()));
                    var updatedVAT = new Price(decimal.Parse(newVAT ?? selectedProduct.TVA?.Value.ToString()));

                    // Calculeaza pretul final corect
                    var finalPriceValue = updatedItemPrice.Value - (updatedItemPrice.Value * updatedVAT.Value / 100);
                    var updatedFinalPrice = new Price(finalPriceValue);

                    var updatedProduct = new CalculatedCartPrice(
                        selectedProduct.CartRegistrationNumber,
                        updatedItemPrice,
                        updatedVAT,
                        updatedFinalPrice)
                    {
                        PriceId = selectedProduct.PriceId
                    };

                    await _pricesRepository.UpdatePriceAsync(updatedProduct);
                    cartPrices[productIndex - 1] = updatedProduct;
                    cartDetails.AppendLine("Product updated successfully.");
                }
                else if (action == "2")
                {
                    await _pricesRepository.DeletePriceAsync(selectedProduct);
                    cartPrices.RemoveAt(productIndex - 1);
                    cartDetails.AppendLine("Product deleted successfully.");
                }
            }
            else
            {
                cartDetails.AppendLine("Invalid option.");
            }

            cartDetails.AppendLine("\nUpdated cart:");
            cartDetails.AppendLine("Nr. | Product | Initial Price | VAT (%) | Final Price");
            cartDetails.AppendLine("---------------------------------------------------");

            foreach (var price in cartPrices)
            {
                cartDetails.AppendLine($"{price.CartRegistrationNumber} | {price.ItemPrice?.Value:0.00} | {price.TVA?.Value:0.00} | {price.FinalPrice?.Value:0.00}");
            }

            return cartDetails.ToString();
        }

        private static string? ReadValue(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine();
        }
    }
}
