using Allegory.Saler.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Allegory.Saler.Items;

public class EfCoreItemStockTransactionRepository
    : EfCoreRepository<SalerDbContext, ItemStockTransaction, int>,
    IItemStockTransactionRepository
{
    public EfCoreItemStockTransactionRepository(
        IDbContextProvider<SalerDbContext> dbContextProvider)
        : base(dbContextProvider)
    {

    }

    public async Task UpdateStatuAsync(
        ItemStockTransactionType type,
        int transactionParentId,
        ItemStockTransactionStatu statu)
    {
        var dbContext = await GetDbContextAsync();
        var entityType = dbContext.Model.FindEntityType(typeof(ItemStockTransaction));
        string tableName = entityType.GetSchema().IsNullOrWhiteSpace() ?
            entityType.GetTableName() :
            entityType.GetSchema() + "." + entityType.GetTableName();

        dbContext.Database.ExecuteSqlRaw(
            @$"UPDATE {tableName}
               SET Statu = {(byte)statu}
               WHERE Type = {(byte)type} AND TransactionParentId = {transactionParentId}");
    }
}
