using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace StockTickR.Repositories.Core
{
    /// The UnitOfWork and repository patterns are intended to act like a abstraction layer between business logic and data access layer.
    /// This can help insulate your application from changes in the data store and can facilitate automated unit testing / test driven development.

    public interface IRepository<T, TId> where T : class
    {
        /// <summary>
        /// Get the specified entity by id.
        /// </summary>
        /// <returns>Entity.</returns>
        /// <param name="id">Identifier of the entity.</param>
        T Get(TId id);

        /// <summary>
        /// Get all entities.
        /// </summary>
        /// <returns>All entities.</returns>
        IEnumerable<T> GetAll();

        /// <summary>
        /// Find the specified entity where the predicate returns true.
        /// </summary>
        /// <returns>Entity where the predicate returns true</returns>
        /// <param name="predicate">predicate that determines if the entity is the one that is searched for.</param>
        T Find(Predicate<T> predicate);

        /// <summary>
        /// Add the specified entity by id.
        /// </summary>
        /// <returns>Success or failure of the operation.</returns>
        /// <param name="entity">Entity to add.</param>
        EntityEntry<T> Add(T entity);

        /// <summary>
        /// Add the specified entity by id.
        /// </summary>
        /// <returns>Success or failure of the operation.</returns>
        /// <param name="entity">Entity to add.</param>
        void AddRange(IEnumerable<T> entity);

        /// <summary>
        /// Remove the specified entity by id.
        /// </summary>
        /// <param name="id">Identifier of the entity.</param>
        EntityEntry<T> Remove(T entity);

        /// <summary>
        /// Remove the specified entity by id.
        /// </summary>
        /// <param name="entities">Entities to remove.</param>
        void RemoveRange(IEnumerable<T> entities);
    }
}
