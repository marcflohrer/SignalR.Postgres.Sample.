using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using StockDatabase.Models;

namespace StockDatabase.Repositories {
    public class StockDbContext : DbContext {
        internal DbSet<Stock> Stocks { get; set; }
        public IConfigurationRoot Configuration { get; }

        public string DefaultSchema = "dbs";

        public StockDbContext (DbContextOptions options, IConfigurationRoot configuration) : base (options) {
            Configuration = configuration;
        }

        protected override void OnModelCreating (ModelBuilder modelBuilder) {
            modelBuilder.HasDefaultSchema (DefaultSchema);

            var msftPrice = 75.12M;
            var applePrice = 158.44M;
            var googlePrice = 1200.96M;
            // Configure model
            modelBuilder.Entity<Stock> (e => {
                e.HasKey (o => o.Id);
                e.HasData (
                    new { Id = 1, Symbol = "MSFT", Price = msftPrice, DayHigh = msftPrice, DayLow = msftPrice, DayOpen = msftPrice, DayClose = msftPrice, LastChange = 0M },
                    new { Id = 2, Symbol = "AAPL", Price = applePrice, DayHigh = applePrice, DayLow = applePrice, DayOpen = applePrice, DayClose = applePrice, LastChange = 0M },
                    new { Id = 3, Symbol = "GOOG", Price = googlePrice, DayHigh = googlePrice, DayLow = googlePrice, DayOpen = googlePrice, DayClose = googlePrice, LastChange = 0M }
                );
            });
        }

        protected override void OnConfiguring (DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.UseNpgsql (string.Format (Configuration.GetConnectionString ("StockDatabase"),
                Configuration["POSTGRES_USER"],
                Configuration["POSTGRES_PASSWORD"],
                Configuration["POSTGRES_DB"]));
        }

    }
}