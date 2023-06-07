using Allegory.Saler.Items;
using Allegory.Saler.Units;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;

namespace Allegory.Saler.Orders.StockHandlers;

internal class StockHandlerManager : ITransientDependency
{
    private readonly IReadOnlyRepository<Unit, int> _unitRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IItemStockTransactionRepository _itemStockTransactionRepository;
    
    public StockHandlerManager(
        IReadOnlyRepository<Unit, int> unitRepository,
        IOrderRepository orderRepository,
        IItemStockTransactionRepository itemStockTransactionRepository)
    {
        _unitRepository = unitRepository;
        _orderRepository = orderRepository;
        _itemStockTransactionRepository = itemStockTransactionRepository;
    }

    public ItemStockTransactionStatu ConvertToStatu(OrderStatu orderStatu)
    {
        switch (orderStatu)
        {
            case OrderStatu.Offer:
            case OrderStatu.Approved:
                return ItemStockTransactionStatu.Reserved;
            case OrderStatu.Closed:
                return ItemStockTransactionStatu.Approved;
            case OrderStatu.Cancelled:
                return ItemStockTransactionStatu.Cancelled;
            default:
                return ItemStockTransactionStatu.Reserved;
        }
    }

    public async Task InsertStockAsync(OrderLine line)
    {
        var order = await _orderRepository.GetAsync(line.OrderId, includeDetails: false);

        var quantity = await GetMainQuantity(line);

        ItemStockTransaction stockTransaction = new ItemStockTransaction(
            line.ProductId,
            quantity,
            line.ReserveDate.Value,
            ItemStockTransactionType.OrderLine,
            line.Id,
            line.OrderId,
            order.Type == OrderType.Sales,
            ConvertToStatu(order.Statu));

        await _itemStockTransactionRepository.InsertAsync(stockTransaction);
    }

    public async Task UpdateStockAsync(OrderLine line)
    {
        var order = await _orderRepository.GetAsync(line.OrderId, includeDetails: false);

        var stockTransaction = await _itemStockTransactionRepository.GetAsync(
            transaction => transaction.Type == ItemStockTransactionType.OrderLine 
            && transaction.TransactionId == line.Id);

        var quantity = await GetMainQuantity(line);

        stockTransaction.ItemId = line.ProductId;
        stockTransaction.SetQuantity(quantity);
        stockTransaction.Date = line.ReserveDate.Value;
        stockTransaction.Statu = ConvertToStatu(order.Statu);

        await _itemStockTransactionRepository.UpdateAsync(stockTransaction);
    }

    public async Task<decimal> GetMainQuantity(OrderLine line)
    {
        var unit = await _unitRepository.GetAsync(line.UnitId, includeDetails: false);

        var quantity = unit.ConvFact2 / unit.ConvFact1 * line.ReserveQuantity.Value;

        return quantity;
    }

    public async Task UpdateStatuAsync(Order order)
    {
        await _itemStockTransactionRepository.UpdateStatuAsync(
            ItemStockTransactionType.OrderLine,
            order.Id,
            ConvertToStatu(order.Statu));
    }
}
