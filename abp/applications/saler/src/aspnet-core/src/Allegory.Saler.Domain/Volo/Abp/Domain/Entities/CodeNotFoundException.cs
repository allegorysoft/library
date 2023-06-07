using System;

namespace Volo.Abp.Domain.Entities;

public class CodeNotFoundException : EntityNotFoundException
{
    public string EntityCode { get; set; }

    public CodeNotFoundException(Type entityType, string entityCode) : base(entityType)
    {
        EntityCode = entityCode;
    }
}
