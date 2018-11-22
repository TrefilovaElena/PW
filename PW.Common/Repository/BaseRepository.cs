using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Common.Data;
using System;
using System.Collections.Generic;

namespace Common.Repository
{

    public class BaseRepository<TDbContext, TEntity> : IRepository<TEntity>
        where TEntity : class, IEntityBase
        where TDbContext : DbContext
    {
        private readonly DbSet<TEntity> _dbSet;
        protected readonly TDbContext _dbContext;

        public BaseRepository(TDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<TEntity>();
        }

        public void InsertRange(IEnumerable<TEntity> entities)
        {
            _dbSet.AddRange(entities);
        }

        public int Count()
        {
            return _dbSet.Count();
        }

        public void Clear()
        {
            _dbSet.RemoveRange(_dbSet);
        }


        public virtual IQueryable<TEntity> GetQuery()
        {
            return _dbSet;
        }

        public virtual IEnumerable<TEntity> GetAll()
        {
            return GetQuery().AsEnumerable();
        }

        public virtual IQueryable<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        {
            return GetQuery().Where(predicate);
        }

        public virtual TEntity Single(int id)
        {
            return GetQuery().Single(e => e.Id == id);
        }

        public virtual TEntity Single(Expression<Func<TEntity, bool>> predicate)
        {
            return GetQuery().Single(predicate);
        }

        public virtual TEntity SingleOrDefault(int id)
        {
            return GetQuery().SingleOrDefault(e => e.Id == id);
        }

        public virtual TEntity SingleOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return GetQuery().SingleOrDefault(predicate);
        }

        public virtual TEntity First(int id)
        {
            return GetQuery().First(e => e.Id == id);
        }

        public virtual TEntity First(Expression<Func<TEntity, bool>> predicate)
        {
            return GetQuery().First(predicate);
        }

        public virtual TEntity FirstOrDefault(int id)
        {
            return GetQuery().FirstOrDefault(e => e.Id == id);
        }

        public virtual TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return GetQuery().FirstOrDefault(predicate);
        }

        public virtual TEntity Add(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            return _dbSet.Add(entity).Entity;
        }

        public virtual TEntity Update(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            var edit = Single(e => e.Id == entity.Id);

            _dbContext.Entry(edit).CurrentValues.SetValues(entity);

            return edit;
        }


        public virtual void ReferenceLoad(TEntity entity, params string[] references)
        {
            foreach (var reference in references)
            {
                _dbContext.Entry(entity).Reference(reference).Load();
            }
        }

        public virtual void Delete(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            _dbSet.Remove(entity);
        }

        public virtual void Attach(TEntity entity)
        {
            _dbSet.Attach(entity);
        }

        public virtual void SaveChanges()
        {
            _dbContext.SaveChanges();
        }

        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_dbContext != null)
                {
                    _dbContext.Dispose();
                }
            }
        }

        public IEnumerable<IEntityBase> GetEntityBases()
        {
            return GetAll();
        }


        public object GetEntitiesByIds(IEnumerable<int> ids)
        {
            return Find(e => ids.Contains(e.Id)).ToList();
        }

        public IEnumerable<TEntity> GetAllIncluding(params Expression<Func<TEntity, object>>[] includeProperties)
        {
            var query = GetQuery();

            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }

            return query.AsEnumerable();
        }

        public IEnumerable<TEntity> FindByIncluding(Expression<Func<TEntity, bool>> predicate,
            params Expression<Func<TEntity, object>>[] includeProperties)
        {
            var query = GetQuery().Where(predicate);

            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }

            return query.AsEnumerable();
        }

        public IQueryable<TEntity> GetQueryByIncluding(params Expression<Func<TEntity, object>>[] includeProperties)
        {
            var query = GetQuery();

            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }

            return query;
        }
    }
}
