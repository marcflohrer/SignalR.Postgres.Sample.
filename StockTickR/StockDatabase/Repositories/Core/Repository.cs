using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using StockDatabase.Models.Core;

namespace StockDatabase.Repositories.Core
{
    public class Repository<T, Tid> : IRepository<T, Tid> where T : BaseEntity
    {
        protected readonly DbContext _dbContext;
        private ILogger<Repository<T, Tid>> _logger;
        public Repository(DbContext dbContext, ILogger<Repository<T, Tid>> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public EntityEntry<T> Add(T entity)
        {
            _logger.LogDebug("Add " + entity);
            return _dbContext.Add<T>(entity);
        }

        public void AddRange(IEnumerable<T> entities)
        {
            _logger.LogDebug("AddRange " + entities);
            _dbContext.AddRange(entities);
        }

        public T Find(Predicate<T> predicate)
        {
            _logger.LogDebug("Find " + predicate);
            return _dbContext.Find<T>(predicate);
        }

        public T Get(Tid id)
        {
            _logger.LogDebug("Get id: " + id);
            return _dbContext.Set<T>().Find(id);
        }

        public IEnumerable<T> GetAll()
        {
            _logger.LogDebug("Get all!");
            return _dbContext.Set<T>().ToListAsync<T>().GetAwaiter().GetResult();
        }

        public EntityEntry<T> Remove(T entity)
        {
            _logger.LogDebug("Remove " + entity);
            return _dbContext.Set<T>().Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            _logger.LogDebug("RemoveRange " + entities);
            _dbContext.Set<T>().RemoveRange(entities);
        }
    }
}
