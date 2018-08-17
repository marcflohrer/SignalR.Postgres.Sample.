using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StockTickR.Models;

namespace StockTickR.Repository
{
    public class StocksRepository : IStocksRepository
    {

        readonly StocksDbContext _context;
        readonly ILogger _logger;

        public StocksRepository(StocksDbContext context, ILoggerFactory loggerFactory)
        {
            _context = context;
            _logger = loggerFactory.CreateLogger("StocksRepository");
        }

        public StocksRepository()
        {
        }

        async Task<List<Stock>> IStocksRepository.GetStocksAsync()
        {
            return await _context.Stocks.OrderBy(c => c.Symbol).ToListAsync();
        }

        async Task<Stock> IStocksRepository.GetStockAsync(string symbol)
        {
            return await _context.Stocks.SingleOrDefaultAsync(c => c.Symbol == symbol);
        }

        async Task<Stock> IStocksRepository.InsertStockAsync(Stock stock)
        {
            _context.Add(stock);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception exp)
            {
                _logger.LogError($"Error in {nameof(IStocksRepository.InsertStockAsync)}: " + exp.Message);
            }

            return stock;
        }

        async Task<bool> IStocksRepository.UpdateStockAsync(Stock stock)
        {
            //Will update all properties of the Stock
            _context.Stocks.Attach(stock);
            _context.Entry(stock).State = EntityState.Modified;
            try
            {
                return (await _context.SaveChangesAsync() > 0);
            }
            catch (Exception exp)
            {
                _logger.LogError($"Error in {nameof(IStocksRepository.UpdateStockAsync)}: " + exp.Message);
            }
            return false;
        }

        async Task<bool> IStocksRepository.DeleteStockAsync(string symbol)
        {
            //Extra hop to the database but keeps it nice and simple for this demo
            var stock = await _context.Stocks.SingleOrDefaultAsync(c => c.Symbol == symbol);
            _context.Remove(stock);
            try
            {
                return (await _context.SaveChangesAsync() > 0);
            }
            catch (Exception exp)
            {
                _logger.LogError($"Error in {nameof(IStocksRepository.DeleteStockAsync)}: " + exp.Message);
            }
            return false;
        }
    }
}
