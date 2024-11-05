using lab3.Repos;
using lab3.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace lab3.Repos
{
    public class CartsRepository: ICartsRepository
    {
        private readonly PricesContext pricesContext;
        public CartsRepository(PricesContext pricesContext)
        {
            this.pricesContext = pricesContext;
        }
        public async Task<List<CartRegistrationNumber>> GetExistingCartsAsync(IEnumerable<string> cartsToCheck)
        {
            List<Models.CartDto> carts = await pricesContext.Carts
                .Where(cart => cartsToCheck.Contains(cart.RegistrationNumber))
                .AsNoTracking()
                .ToListAsync();
            return carts.Select(cart => new CartRegistrationNumber(cart.RegistrationNumber)).ToList();
        }
    
    }   
}