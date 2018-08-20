using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StockDatabase.Models;
using StockDatabase.Repositories.Core;

namespace StockDatabase.Repositories
{
    public interface IStockRepository : IRepository<Stock, string>
    {
        IEnumerable<Stock> GetOrderedStocks();
    }
}
