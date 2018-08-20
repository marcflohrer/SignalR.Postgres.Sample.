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

        public IEnumerable<Stock> GetOrderedStocks()
        {
            return _context.Stocks.ToListAsync().GetAwaiter().GetResult();
        }

        /*async Task<Stock> IStockRepository.GetStockAsync(string symbol)
        {
            return await _context.Stocks.SingleOrDefaultAsync(c => c.Symbol == symbol);
        }

        async Task<Stock> IStockRepository.InsertStockAsync(Stock stock)
        {
            _context.Add(stock);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception exp)
            {
                _logger.LogError($"Error in {nameof(IStockRepository.InsertStockAsync)}: " + exp.Message);
            }

            return stock;
        }

        async Task<bool> IStockRepository.UpdateStockAsync(Stock stock)
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
                _logger.LogError($"Error in {nameof(IStockRepository.UpdateStockAsync)}: " + exp.Message);
            }
            return false;
        }

        async Task<bool> IStockRepository.DeleteStockAsync(string symbol)
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
                _logger.LogError($"Error in {nameof(IStockRepository.DeleteStockAsync)}: " + exp.Message);
            }
            return false;
        }*/
    }
}
