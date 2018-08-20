using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using StockDatabase.Models.Core;

namespace StockDatabase.Repositories.Core
{
    public class Repository<T, Tid> : IRepository<T, Tid> where T : BaseEntity
    {
        protected readonly DbContext _dbContext;
        public Repository(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public EntityEntry<T> Add(T entity)
        {
            return _dbContext.Add<T>(entity);
        }

        public void AddRange(IEnumerable<T> entities)
        {
            _dbContext.AddRange(entities);
        }

        public T Find(Predicate<T> predicate)
        {
            return _dbContext.Find<T>(predicate);
        }

        public T Get(Tid id)
        {
            return _dbContext.Set<T>().Find(id);
        }

        public IEnumerable<T> GetAll()
        {
            return _dbContext.Set<T>().ToListAsync<T>().GetAwaiter().GetResult();
        }

        public EntityEntry<T> Remove(T entity)
        {
            return _dbContext.Set<T>().Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            _dbContext.Set<T>().RemoveRange(entities);
        }
    }
}
