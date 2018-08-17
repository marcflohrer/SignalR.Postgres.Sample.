using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StockTickR.Models;

namespace StockTickR.Repository
{
    public interface IStocksRepository
    {
        Task<List<Stock>> GetStocksAsync();
        Task<Stock> GetStockAsync(string symbol);
        Task<Stock> InsertStockAsync(Stock stock);
        Task<bool> UpdateStockAsync(Stock stock);
        Task<bool> DeleteStockAsync(string symbol);
    }
}
