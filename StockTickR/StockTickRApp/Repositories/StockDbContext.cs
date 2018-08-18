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
            modelBuilder.Entity<Symbol>()
                        .HasKey(b => b.Id);
            modelBuilder.Entity<Symbol>()
                        .HasData(
                            new Symbol { Id = 1, Name = "MSFT" },
                            new Symbol { Id = 2, Name = "AAPL" }, 
                            new Symbol { Id = 3, Name = "GOOG" });

            /*var msftPrice = 75.12M;
            var applePrice = 158.44M;
            var googlePrice = 158.44M;
            // Configure model
            modelBuilder.Entity<Stock>(e =>
            {
                e.HasKey(o => o.Id);
                e.HasData(
                    new { Id = 1, Name = "MSFT", Price = msftPrice, DayHigh = msftPrice, DayLow = msftPrice, DayOpen = msftPrice, DayClose = msftPrice, LastChange = 0M },
                    new { Id = 2, Name = "AAPL", Price = applePrice, DayHigh = applePrice, DayLow = applePrice, DayOpen = applePrice, DayClose = applePrice, LastChange = 0M },
                    new { Id = 3, Name = "GOOG", Price = googlePrice, DayHigh = googlePrice, DayLow = googlePrice, DayOpen = googlePrice, DayClose = googlePrice, LastChange = 0M }
                );
            });*/
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.EnableSensitiveDataLogging();
        //  => optionsBuilder.UseNpgsql("Host=db;Port=5432;Database=stocksdb;User Id=${DB_USER};Password=${DB_PASSWORD};Protocol=3;SSL=true;SslMode=Require;");
    }
}