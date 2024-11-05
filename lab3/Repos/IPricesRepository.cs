using lab3.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace lab3.Repos
{
    public interface IPricesRepository
    {
        Task<List<CalculatedCartPrice>> GetExistingPricesAsync();

        Task SavePricesAsync(Item.PublishedItem prices);
    }
}