using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using StockDatabase.Models.Core;

namespace StockDatabase.Repositories.Core
{
    public class Repository<T, Tid> : IRepository<T, Tid> where T : BaseEntity<Tid>
    {
        protected readonly DbContext _dbContext;
        private ILogger<Repository<T, Tid>> _logger;
        public Repository(DbContext dbContext, ILogger<Repository<T, Tid>> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public virtual EntityEntry<T> Add(T entity)
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
            return _dbContext.Set<T>().AsNoTracking<T>().Where(t => t.Id.Equals(id)).FirstOrDefault();
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
