using lab3.Models;
using lab3.Repos;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using static lab3.Models.Item;
using static lab3.Models.PriceDto;
using static lab3.Models.CartDto;

namespace lab3.Repos
{
  public class PricesRepository : IPricesRepository
  {
    public async Task UpdatePriceAsync(CalculatedCartPrice updatedProduct)
    {
      var existingProduct = await dbContext.Prices
        .FirstOrDefaultAsync(p => p.PriceId == updatedProduct.PriceId);

      if (existingProduct != null)
      {
        existingProduct.Item = updatedProduct.ItemPrice?.Value;
        existingProduct.TVA = updatedProduct.TVA?.Value;
        existingProduct.Final = updatedProduct.FinalPrice?.Value;
        
        await dbContext.SaveChangesAsync();
      }
    }
    public async Task DeletePriceAsync(CalculatedCartPrice productToDelete)
    {
      var existingProduct = await dbContext.Prices
        .FirstOrDefaultAsync(p => p.PriceId == productToDelete.PriceId);

      if (existingProduct != null)
      {
        dbContext.Prices.Remove(existingProduct);
        await dbContext.SaveChangesAsync();
      }
    }

    
    private readonly PricesContext dbContext;

    public PricesRepository(PricesContext dbContext)
    {
      this.dbContext = dbContext;
    }

    public async Task<List<CalculatedCartPrice>> GetExistingPricesAsync()
    {
      //load entities from database
      var foundCartPrices = await (
        from p in dbContext.Prices
        join c in dbContext.Carts on p.CartId equals c.CartId
        select new { c.RegistrationNumber, p.PriceId, p.Item, p.TVA, p.Final }
      ).AsNoTracking()
       .ToListAsync();

      //map database entity to domain model
      List<CalculatedCartPrice> foundPricesModel = foundCartPrices.Select(result =>
        new CalculatedCartPrice(
          CartRegistrationNumber: new CartRegistrationNumber(result.RegistrationNumber),
          ItemPrice: result.Item is null ? null : new Price(result.Item.Value),
          TVA: result.TVA is null ? null : new Price(result.TVA.Value),
          FinalPrice: result.Final is null ? null : new Price(result.Final.Value))
        { 
          PriceId = result.PriceId
        })
         .ToList();

      return foundPricesModel;
    }

    public async Task SavePricesAsync(PublishedItem item)
    {
      ILookup<string, CartDto> carts = (await dbContext.Carts.ToListAsync())
        .ToLookup(cart => cart.RegistrationNumber);
      AddNewPrices(item, carts);
      UpdateExistingPrices(item, carts);
      await dbContext.SaveChangesAsync();
    }

    private void UpdateExistingPrices(PublishedItem item, ILookup<string, CartDto> carts)
    {
      IEnumerable<PriceDto> updatedPrices = item.PriceList.Where(g => g.IsUpdated && g.PriceId > 0)
        .Select(g => new PriceDto()
        {
          PriceId = g.PriceId,
          CartId = carts[g.CartRegistrationNumber.Value].Single().CartId,
          Item = g.ItemPrice?.Value,
          TVA = g.TVA?.Value,
          Final = g.FinalPrice?.Value,
        });

      foreach (PriceDto entity in updatedPrices)
      {
        dbContext.Entry(entity).State = EntityState.Modified;
      }
    }

    private void AddNewPrices(PublishedItem item, ILookup<string, CartDto> carts)
    {
      IEnumerable<PriceDto> newPrices = item.PriceList
        .Where(g => !g.IsUpdated && g.PriceId == 0)
        .Select(g => new PriceDto()
        {
          CartId = carts[g.CartRegistrationNumber.Value].Single().CartId,
          Item = g.ItemPrice?.Value,
          TVA = g.TVA?.Value,
          Final = g.FinalPrice?.Value,
        });
      dbContext.AddRange(newPrices);
    }
  }
}