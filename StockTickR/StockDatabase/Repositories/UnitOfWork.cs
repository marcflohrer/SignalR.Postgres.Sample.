using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using StockDatabase.Models;
using StockDatabase.Repositories.Core;

namespace StockDatabase.Repositories
{
    /// Supports all classes in the .NET Framework class hierarchy and provides low-level services to derived classes. This is the ultimate base class of all classes in the .NET Framework; it is the root of the type hierarchy.
    public class UnitOfWork : IUnitOfWork
    {
        StockDbContext _stockContext { get; }

        public IStockRepository Stocks { get; private set; }

        public UnitOfWork(StockDbContext stockContext, ILogger<Repository<Stock, int>> logger)
        {
            Stocks = new StockRepository(stockContext, logger);
            _stockContext = stockContext;
        }

        public int Complete()
        {
            return _stockContext.SaveChanges();
        }

        public void Dispose()
        {
            _stockContext.Dispose();
        }
    }
}
