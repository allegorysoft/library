using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Allegory.Standart.Entities.Abstract;
using Allegory.Standart.Entities.Concrete;
using Allegory.Standart.EntityRepository.Abstract;

namespace Allegory.Standart.EntityRepository.Concrete
{
    public class InMemoryEntityRepository<TEntity> : EntityRepositoryBase<TEntity> where TEntity : class, IEntity, new()
    {
        public static readonly IList<TEntity> EntityList = new List<TEntity>();
        public override TEntity Get(Expression<Func<TEntity, bool>> filter)
        {
            Console.WriteLine($"{MethodBase.GetCurrentMethod()} : {filter}");
            return filter == null ? EntityList.FirstOrDefault() : EntityList.FirstOrDefault(filter?.Compile());
        }
        public override TEntity GetSingle(Expression<Func<TEntity, bool>> filter)
        {
            Console.WriteLine($"{MethodBase.GetCurrentMethod()} : {filter}");
            return filter == null ? EntityList.Single() : EntityList.Single(filter?.Compile());
        }
        public override List<TEntity> GetList(Expression<Func<TEntity, bool>> filter = null)
        {
            Console.WriteLine($"{MethodBase.GetCurrentMethod()} : {filter}");
            return filter == null ? EntityList.ToList() : EntityList.Where(filter.Compile()).ToList();
        }

        protected override TEntity AddOrm(TEntity entity)
        {
            Console.WriteLine($"{MethodBase.GetCurrentMethod()} : {entity}");
            EntityList.Add(entity);
            return entity;
        }
        protected override TEntity UpdateOrm(TEntity entity)
        {
            Console.WriteLine($"{MethodBase.GetCurrentMethod()} : {entity}");
            return entity;
        }
        public override void Delete(TEntity entity)
        {
            Console.WriteLine($"{MethodBase.GetCurrentMethod()} : {entity}");
            EntityList.Remove(entity);
        }

        protected override List<TEntity> AddOrm(List<TEntity> entities)
        {
            Console.WriteLine($"{MethodBase.GetCurrentMethod()} : {entities}");
            entities.ForEach(entity => EntityList.Add(entity));
            return entities;
        }
        protected override List<TEntity> UpdateOrm(List<TEntity> entities)
        {
            Console.WriteLine($"{MethodBase.GetCurrentMethod()} : {entities}");
            return entities;
        }
        protected override void DeleteOrm(List<TEntity> entities)
        {
            Console.WriteLine($"{MethodBase.GetCurrentMethod()} : {entities}");
            entities.ForEach(entity => EntityList.Remove(entity));
        }

        public override PagedResult<TEntity> GetPagedList<TKey>(Expression<Func<TEntity, TKey>> order, int page = 1, int pageSize = 20, Expression<Func<TEntity, bool>> filter = null, bool desc = false)
        {
            Console.WriteLine($"{MethodBase.GetCurrentMethod()} : {filter}");
            return EntityRepositoryBase.GetPaged(EntityList.AsQueryable(), order, page, pageSize, filter, desc);
        }
    }
    public class InMemoryEntityRepository<TEntity, TKey> : EntityRepositoryBase<TEntity, TKey>
        where TEntity : class, IKey<TKey>, new()
        where TKey : IEquatable<TKey>
    {
        public InMemoryEntityRepository() : base(new InMemoryEntityRepository<TEntity>()) { }
        public override PagedResult<TEntity> GetPagedList(int page = 1, int pageSize = 20, Expression<Func<TEntity, bool>> filter = null, bool desc = false)
        {
            return EntityRepositoryBase.GetPaged(InMemoryEntityRepository<TEntity>.EntityList.AsQueryable(), o => o.Id, page, pageSize, filter, desc);
        }
    }
}
