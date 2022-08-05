using System;

namespace Allegory.Standart.Entities.Abstract
{
    public interface IKey<TKey> : IEntity where TKey : IEquatable<TKey>
    {
        TKey Id { get; set; }
    }
}
