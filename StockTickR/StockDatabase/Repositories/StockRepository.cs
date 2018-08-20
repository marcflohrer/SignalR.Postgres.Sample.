using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StockDatabase.Models;
using StockDatabase.Repositories.Core;

namespace StockDatabase.Repositories
{
    public class StockRepository : Repository<Stock, string>, IStockRepository
    {

        readonly StockDbContext _context;

        public ILogger<Repository<Stock, string>> Logger { get; }

        public StockRepository(StockDbContext stockContext, ILogger<Repository<Stock, string>> logger) : base(stockContext, logger)
        {
            _context = stockContext;
            Logger = logger;
        }

        public Stock Insert(Stock stock)
        {
            Logger.LogDebug("Insert " + stock);
            _context.Add(stock);
            return stock;
        }

        public void Update(Stock stock)
        {
            Logger.LogDebug("Update " + stock);
            _context.Stocks.Attach(stock);
            _context.Entry(stock).State = EntityState.Modified;
        }

        public void Delete(string symbol)
        {
            Logger.LogDebug("Delete " + symbol);
            _context.Remove(Get(symbol));
        }
    }
}
