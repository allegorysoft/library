using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities.Events;
using Volo.Abp.EventBus;

namespace Allegory.Saler.Orders.StockHandlers;

internal class OrderLineCreateHandler
    : ILocalEventHandler<EntityCreatedEventData<OrderLine>>,
      ITransientDependency
{
    private readonly StockHandlerManager _stockHandlerManager;

    public OrderLineCreateHandler(StockHandlerManager stockHandlerManager)
    {
        _stockHandlerManager = stockHandlerManager;
    }

    public async Task HandleEventAsync(EntityCreatedEventData<OrderLine> eventData)
    {
        var line = eventData.Entity;

        if (line.Type == OrderLineType.Item && line.ReserveDate.HasValue)
        {
            await _stockHandlerManager.InsertStockAsync(line);
        }
    }
}
