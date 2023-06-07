using System;

namespace Volo.Abp.Domain.Entities;

public class ThereIsTransactionRecordException : BusinessException
{
    public bool IsDelete { get; set; }

    public Type EntityType { get; set; }

    public Type TransactionEntityType { get; set; }

    public ThereIsTransactionRecordException(
        Type entityType,
        Type transactionEntityType,
        bool isDelete = default)
    {
        EntityType = entityType;
        TransactionEntityType = transactionEntityType;
        IsDelete = isDelete;
    }
}
