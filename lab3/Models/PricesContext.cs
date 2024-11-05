using Microsoft.EntityFrameworkCore;

namespace lab3.Models
{
    public class PricesContext :DbContext
    {
        public PricesContext(DbContextOptions<PricesContext> options) : base(options)
        {
        }
        public DbSet<PriceDto> Prices { get; set; }
        public DbSet<CartDto> Carts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<CartDto>().ToTable("Cart")
                .HasKey(s => s.CartId);
            
            modelBuilder
                .Entity<PriceDto>(entityBuilder =>
                {
                    entityBuilder
                        .Property(g => g.TVA)
                        .HasColumnType("decimal(18, 0)");

                    entityBuilder
                        .Property(g => g.Item)
                        .HasColumnType("decimal(18, 0)");

                    entityBuilder
                        .Property(g => g.Final)
                        .HasColumnType("decimal(18, 0)");

                    entityBuilder
                        .ToTable("Price")
                        .HasKey(s => s.PriceId);
                });
        }
    }   
}