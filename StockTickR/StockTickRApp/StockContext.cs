using Microsoft.EntityFrameworkCore;
using StockTickR;
using StockTickR.Models;

namespace StockTickR
{
    public class StockContext : DbContext
    {
        public StockContext(DbContextOptions<StockContext> options) : base(options) { }

        public DbSet<Stock> Stocks { get; set; }

    }
}