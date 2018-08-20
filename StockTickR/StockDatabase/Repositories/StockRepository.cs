using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using StockDatabase.Models;
using StockDatabase.Repositories.Core;

namespace StockDatabase.Repositories
{
    public class StockRepository : Repository<Stock, string>, IStockRepository
    {

        readonly StockDbContext _context;

        public StockRepository(StockDbContext stockContext) : base(stockContext)
        {
            _context = stockContext;
        }

        public Stock Insert(Stock stock)
        {
            _context.Add(stock);
            return stock;
        }

        public void Update(Stock stock)
        {
            _context.Stocks.Attach(stock);
            _context.Entry(stock).State = EntityState.Modified;
        }

        public void Delete(string symbol)
        {
            _context.Remove(Get(symbol));
        }
    }
}
