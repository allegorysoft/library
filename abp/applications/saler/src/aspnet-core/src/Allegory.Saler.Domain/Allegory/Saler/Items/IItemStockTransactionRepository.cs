using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Allegory.Saler.Items;

public interface IItemStockTransactionRepository : IRepository<ItemStockTransaction, int>
{
    Task UpdateStatuAsync(
        ItemStockTransactionType type,
        int transactionParentId, 
        ItemStockTransactionStatu statu);
}
