using System;

namespace Volo.Abp.Domain.Entities;

public class CodeAlreadyExistsException : BusinessException
{
    public Type EntityType { get; set; }

    public string EntityCode { get; set; }

    public CodeAlreadyExistsException(Type entityType, string entityCode)
    {
        EntityType = entityType;
        EntityCode = entityCode;
    }
}
