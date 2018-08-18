using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StockTickR.Models;
using StockTickR.Repositories.Core;

namespace StockTickR.Repositories
{
    public interface IStockRepository : IRepository<Stock, string>
    {
        IEnumerable<Stock> GetOrderedStocks();
    }
}
