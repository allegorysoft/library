using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities;

namespace Allegory.Saler.Items;

public class ItemStockTransaction : Entity<int>
{
    public int ItemId { get; internal set; }
    public decimal Quantity { get; protected set; }
    public DateTime Date { get; internal set; }
    public ItemStockTransactionType Type { get; internal set; }
    public int TransactionId { get; internal set; }
    public int TransactionParentId { get; internal set; }
    public bool IsOutput { get; internal set; }
    public ItemStockTransactionStatu Statu { get; internal set; }

    protected ItemStockTransaction() { }

    internal ItemStockTransaction(
        int itemId,
        decimal quantity,
        DateTime date,
        ItemStockTransactionType type,
        int transactionId,
        int transactionParentId,
        bool isOutput,
        ItemStockTransactionStatu statu)
    {
        ItemId = itemId;
        SetQuantity(quantity);
        Date = date;
        Type = type;
        TransactionId = transactionId;
        TransactionParentId = transactionParentId;
        IsOutput = isOutput;
        Statu = statu;
    }

    public void SetQuantity(decimal quantity)
    {
        if (quantity <= 0)
            throw new UserFriendlyException("Quantity must be greater than 0");

        Quantity = quantity;
    }
}
