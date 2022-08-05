using System;

namespace Allegory.Standart.Entities.Abstract
{
    public interface ICreatedBy<TKey> : IEntity 
    {
        TKey CreatedBy { get; set; }
    }
}
