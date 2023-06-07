using Allegory.Saler.Calculations.Product;
using Allegory.Saler.Clients;
using Allegory.Saler.Items;
using Allegory.Saler.Services;
using Allegory.Saler.Units;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.EventBus.Local;

namespace Allegory.Saler.Orders;

public class OrderManager : SalerDomainService
{
    protected IOrderRepository OrderRepository { get; }
    protected IReadOnlyRepository<Unit, int> UnitRepository => LazyServiceProvider.LazyGetRequiredService<IReadOnlyRepository<Unit, int>>();
    protected IClientRepository ClientRepository => LazyServiceProvider.LazyGetRequiredService<IClientRepository>();
    protected IItemRepository ItemRepository => LazyServiceProvider.LazyGetRequiredService<IItemRepository>();
    protected IServiceRepository ServiceRepository => LazyServiceProvider.LazyGetRequiredService<IServiceRepository>();
    protected ILocalEventBus LocalEventBus => LazyServiceProvider.LazyGetRequiredService<ILocalEventBus>();
    internal ProductCalculator<OrderDiscount, OrderLine, OrderLineDiscount> ProductCalculator => LazyServiceProvider.LazyGetRequiredService<ProductCalculator<OrderDiscount, OrderLine, OrderLineDiscount>>();

    public OrderManager(IOrderRepository orderRepository)
    {
        OrderRepository = orderRepository;
    }

    public async Task<Order> CreateAsync(
        string number,
        OrderType type,
        DateTime date,
        OrderStatu statu = OrderStatu.Offer,
        string clientCode = default)
    {
        await CheckOrderExistsAsync(number, type);

        var order = new Order(
            number,
            type,
            date,
            statu: statu);

        await SetClientAsync(order, clientCode);

        return order;
    }

    public async Task CheckOrderExistsAsync(
        string number,
        OrderType type,
        int? orderId = default)
    {
        Expression<Func<Order, bool>> expression = order =>
        order.Number == number && order.Type == type;

        if (orderId != default)
            expression = expression.And(order => order.Id != orderId);

        var orderExists = await OrderRepository.AnyAsync(expression);

        if (orderExists)
            throw new NumberAlreadyExistsException(typeof(Order), number);
    }

    public async Task<OrderLine> CreateOrderLineAsync(
        OrderLineType type,
        string productCode,
        decimal quantity,
        string unitCode,
        decimal price = default,
        decimal vatRate = default,
        bool isVatIncluded = default,
        decimal total = default,
        IList<Discount> discounts = default,
        string deductionCode = default,
        short? deductionPart1 = default,
        short? deductionPart2 = default,
        string currencyCode = default,
        decimal? currencyRate = default,
        decimal? currencyPrice = default,
        decimal? currencyTotal = default,
        DateTime? reserveDate = default,
        decimal? reserveQuantity = default)
    {
        var line = new OrderLine();

        await SetOrderLineProductAsync(
            line,
            type,
            productCode,
            quantity,
            unitCode,
            price: price,
            vatRate: vatRate,
            isVatIncluded: isVatIncluded,
            total: total,
            discounts: discounts,
            deductionCode: deductionCode,
            deductionPart1: deductionPart1,
            deductionPart2: deductionPart2,
            currencyCode: currencyCode,
            currencyRate: currencyRate,
            currencyPrice: currencyPrice,
            currencyTotal: currencyTotal,
            reserveDate: reserveDate,
            reserveQuantity: reserveQuantity);

        return line;
    }

    public async Task SetOrderLineProductAsync(
        OrderLine line,
        OrderLineType type,
        string productCode,
        decimal quantity,
        string unitCode,
        decimal price = default,
        decimal vatRate = default,
        bool isVatIncluded = default,
        decimal total = default,
        IList<Discount> discounts = default,
        string deductionCode = default,
        short? deductionPart1 = default,
        short? deductionPart2 = default,
        string currencyCode = default,
        decimal? currencyRate = default,
        decimal? currencyPrice = default,
        decimal? currencyTotal = default,
        DateTime? reserveDate = default,
        decimal? reserveQuantity = default)
    {
        IEntity<int> product = null;
        Unit unit = null;

        switch (type)
        {
            case OrderLineType.Item:
                var item = await ItemRepository.GetByCodeAsync(productCode, false);
                product = item;
                unit = await GetOrderLineUnitAsync(item.UnitGroupId, unitCode);
                break;

            case OrderLineType.Service:
                var service = await ServiceRepository.GetByCodeAsync(productCode, false);
                product = service;
                unit = await GetOrderLineUnitAsync(service.UnitGroupId, unitCode);
                break;
        }

        await ProductCalculator.CalculateLineAsync(
            line,
            quantity,
            price: price,
            vatRate: vatRate,
            isVatIncluded: isVatIncluded,
            total: total,
            discounts: discounts,
            deductionCode: deductionCode,
            deductionPart1: deductionPart1,
            deductionPart2: deductionPart2,
            currencyCode: currencyCode,
            currencyRate: currencyRate,
            currencyPrice: currencyPrice,
            currencyTotal: currencyTotal);
        line.CheckQuantity();
        unit.CheckDivisibility(line.Quantity);

        await SetReserveAsync(
            line,
            unit,
            type,
            reserveDate,
            reserveQuantity,
            product.Id);

        line.Type = type;
        line.ProductId = product.Id;
        line.UnitId = unit.Id;
    }

    protected async Task<Unit> GetOrderLineUnitAsync(
        int unitGroupId,
        string unitCode)
    {
        var unit = await UnitRepository.FirstOrDefaultAsync(unit => unit.UnitGroupId == unitGroupId && unit.Code == unitCode);

        if (unit == null)
            throw new CodeNotFoundException(typeof(Unit), unitCode);

        return unit;
    }

    protected async Task SetReserveAsync(
        OrderLine line,
        Unit unit,
        OrderLineType type,
        DateTime? reserveDate,
        decimal? reserveQuantity,
        int productId)
    {
        if (!await SetReserveByLineTypeAsync(line, type, reserveDate))
            await CheckReserveChangeAsync(line, unit, reserveDate, reserveQuantity, productId);

        line.SetReserve(reserveDate, reserveQuantity);
        if (reserveQuantity.HasValue)
            unit.CheckDivisibility(reserveQuantity.Value);
    }

    protected async Task<bool> SetReserveByLineTypeAsync(
        OrderLine line,
        OrderLineType type,
        DateTime? reserveDate)
    {
        if (line.Id != default && line.Type != type)
        {
            if (line.Type == OrderLineType.Item
                && type != OrderLineType.Item
                && line.ReserveDate.HasValue)
            {
                //Malzemeden hizmete dönmüşse ve hareket kaydı varsa sil
                await LocalEventBus.PublishAsync(new OrderLineReserveChangedEvent
                {
                    OrderLine = line,
                    StatementType = StatementType.Delete
                });
            }
            else if (line.Type != OrderLineType.Item
                && type == OrderLineType.Item
                && reserveDate.HasValue)
            {
                //Hizmetten malzemeye dönmüşse rezerve tarihi varsa hareket kaydı ekle
                await LocalEventBus.PublishAsync(new OrderLineReserveChangedEvent
                {
                    OrderLine = line,
                    StatementType = StatementType.Insert
                });
            }
            return true;
        }

        return false;
    }

    protected async Task CheckReserveChangeAsync(
        OrderLine line,
        Unit unit,
        DateTime? reserveDate,
        decimal? reserveQuantity,
        int productId)
    {
        if (line.Id != default
            && line.Type == OrderLineType.Item
            && line.IsReserveChange(reserveDate, reserveQuantity, unit.Id, productId))
        {
            StatementType statementType = StatementType.Update;

            if (line.ReserveDate.HasValue && !reserveDate.HasValue)
                statementType = StatementType.Delete;

            else if (!line.ReserveDate.HasValue && reserveDate.HasValue)
                statementType = StatementType.Insert;

            await LocalEventBus.PublishAsync(new OrderLineReserveChangedEvent
            {
                OrderLine = line,
                StatementType = statementType
            });
        }
    }

    public async Task ChangeNumberAsync(
        Order order,
        string newNumber)
    {
        await CheckOrderExistsAsync(newNumber, order.Type, order.Id);
        order.ChangeNumber(newNumber);
    }

    public async Task SetClientAsync(
        Order order,
        string clientCode)
    {
        var client = string.IsNullOrWhiteSpace(clientCode)
            ? null
            : await ClientRepository.GetByCodeAsync(clientCode, includeDetails: false);

        order.ClientId = client?.Id;
    }

    public async Task CalculateTotals(
        Order order,
        IList<Discount> discounts = default,
        string currencyCode = default,
        decimal? currencyRate = default)
    {
        await ProductCalculator.SetCurrencyInfoAsync(
            order,
            currencyCode,
            currencyRate);

        ProductCalculator.CalculateAggregateRoot(
            order,
            discounts);
        await Task.CompletedTask;
    }
}
