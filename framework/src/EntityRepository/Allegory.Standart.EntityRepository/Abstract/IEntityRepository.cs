using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Allegory.Standart.Entities.Abstract;
using Allegory.Standart.Entities.Concrete;

namespace Allegory.Standart.EntityRepository.Abstract
{
    public interface IEntityRepository<TEntiy> where TEntiy : class, IEntity, new()
    {
        TEntiy Get(Expression<Func<TEntiy, bool>> filter);
        TEntiy GetSingle(Expression<Func<TEntiy, bool>> filter);
        List<TEntiy> GetList(Expression<Func<TEntiy, bool>> filter = null);

        TEntiy Add(TEntiy entity);
        TEntiy Update(TEntiy entity);
        void Delete(TEntiy entity);

        List<TEntiy> Add(List<TEntiy> entities);
        List<TEntiy> Update(List<TEntiy> entities);
        void Delete(List<TEntiy> entities);

        PagedResult<TEntiy> GetPagedList<TKey>(Expression<Func<TEntiy, TKey>> order, int page = 1, int pageSize = 20
                                        , Expression<Func<TEntiy, bool>> filter = null, bool desc = false);
    }
    public interface IEntityRepository<TEntity, TKey> : IEntityRepository<TEntity>
       where TEntity : class, IKey<TKey>, new()
       where TKey : IEquatable<TKey>
    {
        TEntity GetById(TKey id);
        List<TEntity> GetById(HashSet<TKey> ids);

        TEntity AddOrUpdate(TEntity entity);
        List<TEntity> AddOrUpdate(List<TEntity> entities);
        void DeleteById(TKey id);
        void DeleteById(HashSet<TKey> ids);

        PagedResult<TEntity> GetPagedList(int page = 1, int pageSize = 20, Expression<Func<TEntity, bool>> filter = null, bool desc = false);
    }
}
