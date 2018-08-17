using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using StockTickR.Models;

namespace StockTickR.Repository
{
    public class StocksDbContext : DbContext
    {
        internal DbSet<Stock> Stocks { get; set; }

        public StocksDbContext(DbContextOptions options) : base(options)
        {
        }

        protected StocksDbContext()
        {
        }
    }
}