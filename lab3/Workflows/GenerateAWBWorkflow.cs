using System;
using System.Threading.Tasks;
using lab3.Models;

namespace lab3.Workflows
{
    public class GenerateAWBWorkflow
    {
        public Task<(string Address, string AWB)> ExecuteAsync()
        {
            // Obtine adresa de la utilizator
            Console.Write("Enter the delivery address: ");
            string? address = Console.ReadLine();

            if (string.IsNullOrEmpty(address))
            {
                return Task.FromResult<(string, string)>(
                    ("Address not provided", "AWB not generated"));
            }

            // Generare AWB unic
            string awb = Guid.NewGuid().ToString().Substring(0, 8).ToUpper();

            return Task.FromResult((address, awb));
        }
    }
}