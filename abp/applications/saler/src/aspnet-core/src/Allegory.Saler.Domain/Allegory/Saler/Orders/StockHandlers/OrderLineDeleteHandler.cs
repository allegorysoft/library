using Allegory.Saler.Items;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities.Events;
using Volo.Abp.EventBus;

namespace Allegory.Saler.Orders.StockHandlers;

public class OrderLineDeleteHandler
    : ILocalEventHandler<EntityDeletedEventData<OrderLine>>,
      ITransientDependency
{
    private readonly IItemStockTransactionRepository _itemStockTransactionRepository;

    public OrderLineDeleteHandler(
        IItemStockTransactionRepository itemStockTransactionRepository)
    {
        _itemStockTransactionRepository = itemStockTransactionRepository;
    }

    public async Task HandleEventAsync(EntityDeletedEventData<OrderLine> eventData)
    {
        if (eventData.Entity.Type == OrderLineType.Item 
            && eventData.Entity.ReserveDate.HasValue)
        {
            await _itemStockTransactionRepository.DeleteAsync(
                i => i.Type == ItemStockTransactionType.OrderLine
                && i.TransactionId == eventData.Entity.Id);
        }
    }
}
