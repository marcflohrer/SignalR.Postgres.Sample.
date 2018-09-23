using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StockDatabase.Models;
using StockDatabase.Repositories.Core;

namespace StockDatabase.Repositories {
    public interface IStockRepository : IRepository<Stock, int> {
        Stock Insert (Stock stock);
        void Update (Stock stock);
        void Delete (int symbol);
        Stock GetStockBySymbol (string symbol);
    }
}