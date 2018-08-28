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

            modelBuilder.Entity<Stock> (e => {
                e.HasKey (o => o.Id);
                e.Property (b => b.Symbol).HasColumnType ("varchar(256)");
                e.Property (b => b.Price).HasColumnType ("decimal(10, 2)");
                e.Property (b => b.DayLow).HasColumnType ("decimal(10, 2)");
                e.Property (b => b.DayOpen).HasColumnType ("decimal(10, 2)");
                e.Property (b => b.DayHigh).HasColumnType ("decimal(10, 2)");
                e.Property (b => b.LastChange).HasColumnType ("decimal(10, 2)");
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