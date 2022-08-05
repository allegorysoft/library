using System.Linq;
using Allegory.Standart.Entities.Abstract;

namespace Allegory.Standart.EntityRepository.Abstract
{
    public interface IQueryableRepository<T> where T : class, IEntity, new()
    {
        IQueryable<T> Table { get; }
    }
}
