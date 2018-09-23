using System;

namespace StockDatabase.Repositories.Interfaces {
    /// The UnitOfWork and repository patterns are intended to act like a abstraction layer between business logic and data access layer.
    /// This can help insulate your application from changes in the data store and can facilitate automated unit testing / test driven development.
    public interface IUnitOfWork : IDisposable {
        IStockRepository Stocks { get; }
        int Complete ();
    }
}