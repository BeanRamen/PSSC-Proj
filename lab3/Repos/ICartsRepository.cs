using lab3.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace lab3.Repos
{
    public interface ICartsRepository
    {
        Task<List<CartRegistrationNumber>> GetExistingCartsAsync(IEnumerable<string> cartsToCheck);
    }
}