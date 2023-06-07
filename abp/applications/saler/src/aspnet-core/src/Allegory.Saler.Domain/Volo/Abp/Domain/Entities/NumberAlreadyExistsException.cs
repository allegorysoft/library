using System;

namespace Volo.Abp.Domain.Entities;

public class NumberAlreadyExistsException : BusinessException
{
    public Type EntityType { get; set; }

    public string EntityNumber { get; set; }

    public NumberAlreadyExistsException(Type entityType, string entityNumber)
    {
        EntityType = entityType;
        EntityNumber = entityNumber;
    }
}
