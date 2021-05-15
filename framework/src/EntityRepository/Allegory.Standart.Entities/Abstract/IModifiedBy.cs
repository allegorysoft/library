using System;

namespace Allegory.Standart.Entities.Abstract
{
    public interface IModifiedBy<TKey> : IEntity 
    {
        TKey ModifiedBy { get; set; }
    }
}
