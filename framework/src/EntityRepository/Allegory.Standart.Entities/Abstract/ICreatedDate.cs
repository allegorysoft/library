using System;

namespace Allegory.Standart.Entities.Abstract
{
    public interface ICreatedDate : IEntity
    {
        DateTime CreatedDate { get; set; }
    }
}
