using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StockDatabase.Models;
using StockDatabase.Repositories.Core;

namespace StockDatabase.Repositories
{
    public class StockRepository : Repository<Stock, int>, IStockRepository
    {

        readonly StockDbContext _context;

        private Serilog.ILogger  _logger { get; }

        public StockRepository(StockDbContext stockContext, Serilog.ILogger  logger) : base(stockContext, logger)
        {
            _context = stockContext;
            _logger = logger;
        }

        public Stock Insert(Stock stock)
        {
            _logger.Debug("Insert " + stock);
            _context.Add(stock);
            return stock;
        }

        public void Update(Stock stock)
        {
            _logger.Debug("Update " + stock.Symbol + " " + stock.Id);
            _context.Stocks.Update(stock);
            _context.Entry(stock).State = EntityState.Modified;
        }

        public void Delete(int id)
        {
            _logger.Debug("Delete " + id);
            _context.Remove(Get(id));
        }
    }
}
