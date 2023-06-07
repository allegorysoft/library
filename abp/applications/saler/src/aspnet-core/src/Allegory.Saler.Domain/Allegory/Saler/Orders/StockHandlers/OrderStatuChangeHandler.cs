using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;

namespace Allegory.Saler.Orders.StockHandlers;

internal class OrderStatuChangeHandler
    : ILocalEventHandler<OrderStatuChangedEvent>,
      ITransientDependency
{
    private readonly StockHandlerManager _stockHandlerManager;

    public OrderStatuChangeHandler(StockHandlerManager stockHandlerManager)
    {
        _stockHandlerManager = stockHandlerManager;
    }

    public async Task HandleEventAsync(OrderStatuChangedEvent eventData)
    {
        await _stockHandlerManager.UpdateStatuAsync(eventData.Order);
    }
}
