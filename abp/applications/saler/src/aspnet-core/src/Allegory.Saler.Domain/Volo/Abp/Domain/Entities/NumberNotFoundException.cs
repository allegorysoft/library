using System;

namespace Volo.Abp.Domain.Entities;

public class NumberNotFoundException : EntityNotFoundException
{
    public string EntityNumber { get; set; }
    public NumberNotFoundException(Type entityType, string entityNumber) : base(entityType)
    {
        EntityNumber = entityNumber;
    }
}
