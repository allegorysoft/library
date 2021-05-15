using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Transactions;
using Allegory.Standart.Entities.Abstract;
using Allegory.Standart.Entities.Concrete;

namespace Allegory.Standart.EntityRepository.Abstract
{
    public abstract class EntityRepositoryBase
    {
        public static PagedResult<T> GetPaged<T>(IQueryable<T> query, int page, int pageSize) where T : class, new()
        {
            var result = new PagedResult<T>();
            result.CurrentPage = page;
            result.PageSize = pageSize;
            result.RowCount = query.Count();

            var pageCount = (double)result.RowCount / pageSize;
            result.PageCount = (int)Math.Ceiling(pageCount);

            var skip = (page - 1) * pageSize;
            result.Results = query.Skip(skip).Take(pageSize).ToList();

            return result;
        }
        public static PagedResult<T> GetPaged<T, TKey>(IQueryable<T> query, Expression<Func<T, TKey>> order, int page = 1, int pageSize = 20, Expression<Func<T, bool>> filter = null, bool desc = false) where T : class, new()
        {
            if (filter != null)
                query = query.Where(filter);
            if (desc)
                query = query.OrderByDescending(order);
            else
                query = query.OrderBy(order);

            return GetPaged(query, page, pageSize);
        }
    }
    public abstract class EntityRepositoryBase<TEntity> : IEntityRepository<TEntity> where TEntity : class, IEntity, new()
    {
        protected bool IsAssignableFromICreatedDate, IsAssignableFromIModifiedDate, IsAssignableFromICreatedBy, IsAssignableFromIModifiedBy;
        public EntityRepositoryBase()
        {
            IsAssignableFromICreatedDate = typeof(ICreatedDate).IsAssignableFrom(typeof(TEntity));
            IsAssignableFromIModifiedDate = typeof(IModifiedDate).IsAssignableFrom(typeof(TEntity));
            IsAssignableFromICreatedBy = typeof(TEntity).GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICreatedBy<>));
            IsAssignableFromIModifiedBy = typeof(TEntity).GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IModifiedBy<>));
        }

        public abstract TEntity Get(Expression<Func<TEntity, bool>> filter);
        public abstract TEntity GetSingle(Expression<Func<TEntity, bool>> filter);
        public abstract List<TEntity> GetList(Expression<Func<TEntity, bool>> filter = null);

        protected abstract TEntity AddOrm(TEntity entity);
        protected abstract TEntity UpdateOrm(TEntity entity);
        public abstract void Delete(TEntity entity);

        protected abstract List<TEntity> AddOrm(List<TEntity> entities);
        protected abstract List<TEntity> UpdateOrm(List<TEntity> entities);
        protected abstract void DeleteOrm(List<TEntity> entities);

        public abstract PagedResult<TEntity> GetPagedList<TKey>(Expression<Func<TEntity, TKey>> order, int page = 1, int pageSize = 20, Expression<Func<TEntity, bool>> filter = null, bool desc = false);

        public TEntity Add(TEntity entity)
        {
            SetForAdd(entity);
            return AddOrm(entity);
        }
        public TEntity Update(TEntity entity)
        {
            SetForUpdate(entity);
            return UpdateOrm(entity);
        }

        public List<TEntity> Add(List<TEntity> entities)
        {
            if (entities == null || entities.Count == 0)
                return entities;
            SetForAdd(entities);
            return AddOrm(entities);
        }
        public List<TEntity> Update(List<TEntity> entities)
        {
            if (entities == null || entities.Count == 0)
                return entities;
            SetForUpdate(entities);
            return UpdateOrm(entities);
        }
        public void Delete(List<TEntity> entities)
        {
            if (entities == null || entities.Count == 0)
                return;
            DeleteOrm(entities);
        }

        protected void SetForAdd(TEntity entity)
        {
            if (IsAssignableFromICreatedDate)
                ((ICreatedDate)entity).CreatedDate = DateTime.Now;
            if (IsAssignableFromIModifiedDate)
                ((IModifiedDate)entity).ModifiedDate = null;
            if (IsAssignableFromICreatedBy)
            {
                var property = entity.GetType().GetProperty(nameof(ICreatedBy<object>.CreatedBy));
                var value = Convert.ChangeType(ClaimsPrincipal.Current.FindFirst(ClaimTypes.NameIdentifier).Value, property.PropertyType);
                property.SetValue(entity, value);
            }
            if (IsAssignableFromIModifiedBy)
                entity.GetType().GetProperty(nameof(IModifiedBy<object>.ModifiedBy)).SetValue(entity, null);
        }
        protected void SetForAdd(List<TEntity> entities)
        {
            if (IsAssignableFromICreatedDate)
                entities.ForEach(X => { (X as ICreatedDate).CreatedDate = DateTime.Now; });
            if (IsAssignableFromIModifiedDate)
                entities.ForEach(X => { (X as IModifiedDate).ModifiedDate = null; });
            if (IsAssignableFromICreatedBy)
            {
                var property = entities.First().GetType().GetProperty(nameof(ICreatedBy<object>.CreatedBy));
                var value = Convert.ChangeType(ClaimsPrincipal.Current.FindFirst(ClaimTypes.NameIdentifier).Value
                                             , property.PropertyType);
                entities.ForEach(entity =>
                {
                    property.SetValue(entity, value);
                });
            }
            if (IsAssignableFromIModifiedBy)
                entities.ForEach(entity => { entity.GetType().GetProperty(nameof(IModifiedBy<object>.ModifiedBy)).SetValue(entity, null); });
        }
        protected void SetForUpdate(TEntity entity)
        {
            if (IsAssignableFromIModifiedDate)
                ((IModifiedDate)entity).ModifiedDate = DateTime.Now;
            if (IsAssignableFromIModifiedBy)
            {
                var property = entity.GetType().GetProperty(nameof(IModifiedBy<object>.ModifiedBy));
                var value = Convert.ChangeType(ClaimsPrincipal.Current.FindFirst(ClaimTypes.NameIdentifier).Value
                                             , Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType);
                property.SetValue(entity, value);
            }
        }
        protected void SetForUpdate(List<TEntity> entities)
        {
            if (IsAssignableFromIModifiedDate)
                entities.ForEach(X => { (X as IModifiedDate).ModifiedDate = DateTime.Now; });
            if (IsAssignableFromIModifiedBy)
            {
                var property = entities.First().GetType().GetProperty(nameof(IModifiedBy<object>.ModifiedBy));
                var value = Convert.ChangeType(ClaimsPrincipal.Current.FindFirst(ClaimTypes.NameIdentifier).Value
                                             , Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType);
                entities.ForEach(entity =>
                {
                    property.SetValue(entity, value);
                });
            }

        }
    }
    public abstract class EntityRepositoryBase<TEntity, TKey> : IEntityRepository<TEntity, TKey>
        where TEntity : class, IKey<TKey>, new()
        where TKey : IEquatable<TKey>
    {   
        #region EntityRepositoryBase
        protected EntityRepositoryBase<TEntity> EntityRepository { get; private set; }
        protected EntityRepositoryBase(EntityRepositoryBase<TEntity> entityRepositoryBase)
        {
            EntityRepository = entityRepositoryBase;
        }

        public TEntity Get(Expression<Func<TEntity, bool>> filter) => EntityRepository.Get(filter);
        public TEntity GetSingle(Expression<Func<TEntity, bool>> filter) => EntityRepository.GetSingle(filter);
        public List<TEntity> GetList(Expression<Func<TEntity, bool>> filter = null) => EntityRepository.GetList(filter);

        public TEntity Add(TEntity entity) => EntityRepository.Add(entity);
        public TEntity Update(TEntity entity) => EntityRepository.Update(entity);
        public void Delete(TEntity entity) => EntityRepository.Delete(entity);

        public List<TEntity> Add(List<TEntity> entities) => EntityRepository.Add(entities);
        public List<TEntity> Update(List<TEntity> entities) => EntityRepository.Update(entities);
        public void Delete(List<TEntity> entities) => EntityRepository.Delete(entities);

        public PagedResult<TEntity> GetPagedList<TOrder>(Expression<Func<TEntity, TOrder>> order, int page = 1, int pageSize = 20, Expression<Func<TEntity, bool>> filter = null, bool desc = false) => EntityRepository.GetPagedList(order, page, pageSize, filter, desc);
        #endregion

        public TEntity GetById(TKey id) => GetSingle(f => f.Id.Equals(id));
        public List<TEntity> GetById(HashSet<TKey> ids) => GetList(f => ids.Contains(f.Id));

        public TEntity AddOrUpdate(TEntity entity)
        {
            if (default(TKey).Equals(entity.Id))
                return Add(entity);
            return Update(entity);
        }
        public List<TEntity> AddOrUpdate(List<TEntity> entities)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, new System.TimeSpan(0, 15, 0)))
            {
                try
                {
                    Update(entities.Where(x => !x.Id.Equals(default(TKey))).ToList());
                    Add(entities.Where(x => x.Id.Equals(default(TKey))).ToList());
                    scope.Complete();
                    return entities;
                }
                catch (Exception ex)
                {
                    scope.Dispose();
                    throw ex;
                }
            }
        }
        public void DeleteById(TKey id) => Delete(GetById(id));
        public void DeleteById(HashSet<TKey> ids) => Delete(GetById(ids));

        public abstract PagedResult<TEntity> GetPagedList(int page = 1, int pageSize = 20, Expression<Func<TEntity, bool>> filter = null, bool desc = false);
    }
}
