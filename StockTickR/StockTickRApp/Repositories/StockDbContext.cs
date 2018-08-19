using Microsoft.EntityFrameworkCore;
using StockTickR.Models;

namespace StockTickR.Repositories
{
    public class StockDbContext : DbContext
    {
        internal DbSet<Stock> Stocks { get; set; }

        public StockDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var msftPrice = 75.12M;
            var applePrice = 158.44M;
            var googlePrice = 158.44M;
            // Configure model
            modelBuilder.Entity<Stock>(e =>
            {
                e.HasKey(o => o.Id);
                e.HasData(
                    new { Id = 1, Symbol = "MSFT", Price = msftPrice, DayHigh = msftPrice, DayLow = msftPrice, DayOpen = msftPrice, DayClose = msftPrice, LastChange = 0M },
                    new { Id = 2, Symbol = "AAPL", Price = applePrice, DayHigh = applePrice, DayLow = applePrice, DayOpen = applePrice, DayClose = applePrice, LastChange = 0M },
                    new { Id = 3, Symbol = "GOOG", Price = googlePrice, DayHigh = googlePrice, DayLow = googlePrice, DayOpen = googlePrice, DayClose = googlePrice, LastChange = 0M }
                );
            });
        }
    }
}