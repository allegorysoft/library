using Allegory.Saler.Items;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;

namespace Allegory.Saler.Orders.StockHandlers;

internal class OrderLineReserveChangeHandler
    : ILocalEventHandler<OrderLineReserveChangedEvent>,
      ITransientDependency
{
    private readonly StockHandlerManager _stockHandlerManager;
    private readonly IItemStockTransactionRepository _itemStockTransactionRepository;

    public OrderLineReserveChangeHandler(
        StockHandlerManager stockHandlerManager,
        IItemStockTransactionRepository itemStockTransactionRepository) 
    {
        _stockHandlerManager = stockHandlerManager;
        _itemStockTransactionRepository = itemStockTransactionRepository;
    }

    public async Task HandleEventAsync(OrderLineReserveChangedEvent eventData)
    {
        switch (eventData.StatementType)
        {
            case System.Data.StatementType.Insert:
                await StockTransactionInsertAsync(eventData.OrderLine);
                break;
            case System.Data.StatementType.Update:
                await StockTransactionUpdateAsync(eventData.OrderLine);
                break;
            case System.Data.StatementType.Delete:
                await StockTransactionDeleteAsync(eventData.OrderLine);
                break;
        }
    }

    public async Task StockTransactionInsertAsync(OrderLine line)
    {
        await _stockHandlerManager.InsertStockAsync(line);
    }

    public async Task StockTransactionUpdateAsync(OrderLine line)
    {
        await _stockHandlerManager.UpdateStockAsync(line);
    }

    public async Task StockTransactionDeleteAsync(OrderLine line)
    {
        await _itemStockTransactionRepository.DeleteAsync(
              i => i.Type == ItemStockTransactionType.OrderLine
              && i.TransactionId == line.Id);
    }
}
