using System;
using Allegory.Standart.Entities.Abstract;

namespace Allegory.NET.EntityRepository.Tests.Setup.Entities
{
    public abstract class EntityBase : IKey<int>, ICreatedDate, IModifiedDate, IActive, ICreatedBy<int>, IModifiedBy<Nullable<int>>
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool Active { get; set; }
        public int CreatedBy { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
    }
}
