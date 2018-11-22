using Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Common
{
    public interface IRepository<TEntity> : IDisposable, IRepository
       where TEntity : class, IEntityBase
    {
        IQueryable<TEntity> GetQuery();
        IEnumerable<TEntity> GetAll();
        IQueryable<TEntity> Find(Expression<Func<TEntity, bool>> predicate);
        TEntity Single(Expression<Func<TEntity, bool>> predicate);
        TEntity Single(int id);
        TEntity SingleOrDefault(Expression<Func<TEntity, bool>> predicate);
        TEntity SingleOrDefault(int id);
        TEntity First(Expression<Func<TEntity, bool>> predicate);
        TEntity First(int id);
        TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate);
        TEntity FirstOrDefault(int id);
        TEntity Add(TEntity entity);
        TEntity Update(TEntity entity);
        int Count();
        void Clear();
        void InsertRange(IEnumerable<TEntity> entities);
        void ReferenceLoad(TEntity entity, params string[] reference);
        void Delete(TEntity entity);
        void Attach(TEntity entity);
        void SaveChanges();
        IQueryable<TEntity> GetQueryByIncluding(params Expression<Func<TEntity, object>>[] includeProperties);
        IEnumerable<TEntity> GetAllIncluding(params Expression<Func<TEntity, object>>[] includeProperties);
        IEnumerable<TEntity> FindByIncluding(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includeProperties);
    }

    public interface IRepository
    {
        object GetEntitiesByIds(IEnumerable<int> ids);
        IEnumerable<IEntityBase> GetEntityBases();
    }
}