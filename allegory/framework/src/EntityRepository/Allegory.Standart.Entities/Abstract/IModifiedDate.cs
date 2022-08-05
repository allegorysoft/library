using System;

namespace Allegory.Standart.Entities.Abstract
{
    public interface IModifiedDate : IEntity
    {
        Nullable<DateTime> ModifiedDate { get; set; }
    }
}
