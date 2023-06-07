using Allegory.Saler.Calculations.Product;
using Allegory.Saler.Clients;
using Allegory.Saler.Currencies;
using Allegory.Saler.Items;
using Allegory.Saler.Permissions;
using Allegory.Saler.Services;
using Allegory.Saler.Units;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.ObjectExtending;

namespace Allegory.Saler.Orders;

[Authorize(SalerPermissions.OrderManagement.Order.Default)]
public class OrderAppService : SalerAppService, IOrderAppService
{
    protected IOrderRepository OrderRepository { get; }
    protected OrderManager OrderManager { get; }
    protected IClientRepository ClientRepository => LazyServiceProvider.LazyGetRequiredService<IClientRepository>();
    protected ICurrencyRepository CurrencyRepository => LazyServiceProvider.LazyGetRequiredService<ICurrencyRepository>();
    protected IServiceRepository ServiceRepository => LazyServiceProvider.LazyGetRequiredService<IServiceRepository>();
    protected IItemRepository ItemRepository => LazyServiceProvider.LazyGetRequiredService<IItemRepository>();
    protected IUnitGroupRepository UnitGroupRepository => LazyServiceProvider.LazyGetRequiredService<IUnitGroupRepository>();
    protected IReadOnlyRepository<Unit, int> UnitRepository => LazyServiceProvider.LazyGetRequiredService<IReadOnlyRepository<Unit, int>>();
    protected IReadOnlyRepository<OrderLine, int> OrderLineRepository => LazyServiceProvider.LazyGetRequiredService<IReadOnlyRepository<OrderLine, int>>();
    protected IReadOnlyRepository<OrderLineDiscount, int> OrderLineDiscountRepository => LazyServiceProvider.LazyGetRequiredService<IReadOnlyRepository<OrderLineDiscount, int>>();

    public OrderAppService(
        IOrderRepository orderRepository,
        OrderManager orderManager)
    {
        OrderRepository = orderRepository;
        OrderManager = orderManager;
    }

    protected async Task<IQueryable<OrderDto>> CreateDtoQueryAsync()
    {
        var query = from order in await OrderRepository.GetQueryableAsync()

                    join client in await ClientRepository.GetQueryableAsync()
                    on order.ClientId equals client.Id into clients
                    from client in clients.DefaultIfEmpty()

                    join currency in await CurrencyRepository.GetQueryableAsync()
                    on order.CurrencyId equals currency.Id into currencies
                    from currency in currencies.DefaultIfEmpty()

                    select new OrderDto
                    {
                        Id = order.Id,
                        Number = order.Number,
                        Type = order.Type,
                        Date = order.Date,
                        Statu = order.Statu,
                        TotalGross = order.TotalGross,
                        ClientCode = client.Code,
                        //ExtraProperties = item.ExtraProperties, => protected set
                        CreatorId = order.CreatorId,
                        CreationTime = order.CreationTime,
                        LastModifierId = order.LastModifierId,
                        LastModificationTime = order.LastModificationTime,
                        CurrencyTotalGross = order.CurrencyRate.HasValue
                        ? Math.Round(order.TotalGross / order.CurrencyRate.Value, 2)
                        : null,
                        CurrencySymbol = currency.Symbol,
                    };

        return query;
    }
    protected async Task<IQueryable<OrderWithDetailsDto>> CreateDetailsDtoQueryAsync()
    {
        var query = from order in await OrderRepository.GetQueryableAsync()

                    join client in await ClientRepository.GetQueryableAsync() on order.ClientId equals client.Id into clients
                    from client in clients.DefaultIfEmpty()

                    join currency in await CurrencyRepository.GetQueryableAsync() on order.CurrencyId equals currency.Id into currencies
                    from currency in currencies.DefaultIfEmpty()

                    select new OrderWithDetailsDto
                    {
                        Id = order.Id,
                        Number = order.Number,
                        Type = order.Type,
                        Date = order.Date,
                        Statu = order.Statu,
                        ClientCode = client.Code,
                        //ExtraProperties = item.ExtraProperties, => protected set
                        CreatorId = order.CreatorId,
                        CreationTime = order.CreationTime,
                        LastModifierId = order.LastModifierId,
                        LastModificationTime = order.LastModificationTime,
                        TotalDiscount = order.TotalDiscount,
                        TotalVatBase = order.TotalVatBase,
                        TotalVatAmount = order.TotalVatAmount,
                        TotalGross = order.TotalGross,
                        Discounts = ObjectMapper.Map<IList<OrderDiscount>, IList<DiscountDto>>(order.Discounts),
                        CurrencyCode = currency.Code,
                        CurrencyRate = order.CurrencyRate,
                    };

        return query;
    }
    protected async Task<OrderWithDetailsDto> FindAsync(Expression<Func<OrderWithDetailsDto, bool>> predicate)
    {
        var query = await CreateDetailsDtoQueryAsync();
        return await AsyncExecuter.FirstOrDefaultAsync(query.Where(predicate));
    }
    protected async Task FillLineDetailsDtoAsync(OrderWithDetailsDto input)
    {
        var query = from orderLine in await OrderLineRepository.GetQueryableAsync()

                    join unit in await UnitRepository.GetQueryableAsync()
                    on orderLine.UnitId equals unit.Id

                    join unitGroup in await UnitGroupRepository.GetQueryableAsync()
                    on unit.UnitGroupId equals unitGroup.Id

                    join item in await ItemRepository.GetQueryableAsync()
                    on new
                    {
                        ProductId = orderLine.ProductId,
                        Type = orderLine.Type
                    }
                    equals new
                    {
                        ProductId = item.Id,
                        Type = OrderLineType.Item
                    } into items
                    from item in items.DefaultIfEmpty()

                    join service in await ServiceRepository.GetQueryableAsync()
                    on new
                    {
                        ProductId = orderLine.ProductId,
                        Type = orderLine.Type
                    }
                    equals new
                    {
                        ProductId = service.Id,
                        Type = OrderLineType.Service
                    } into services
                    from service in services.DefaultIfEmpty()

                    join currency in await CurrencyRepository.GetQueryableAsync()
                    on orderLine.CurrencyId equals currency.Id into currencies
                    from currency in currencies.DefaultIfEmpty()

                    where orderLine.OrderId == input.Id

                    select new OrderLineDto
                    {
                        Id = orderLine.Id,
                        Type = orderLine.Type,
                        ProductCode = orderLine.Type == OrderLineType.Item ? item.Code : service.Code,
                        ProductName = orderLine.Type == OrderLineType.Item ? item.Name : service.Name,
                        UnitGroupCode = unitGroup.Code,
                        UnitCode = unit.Code,
                        Quantity = orderLine.Quantity,
                        Price = orderLine.Price,
                        VatRate = orderLine.VatRate,
                        IsVatIncluded = orderLine.IsVatIncluded,
                        VatBase = orderLine.VatBase,
                        VatAmount = orderLine.VatAmount,
                        Total = orderLine.Total,
                        CalculatedTotal = orderLine.CalculatedTotal,
                        DiscountTotal = orderLine.DiscountTotal,
                        Discounts = ObjectMapper.Map<IList<OrderLineDiscount>, IList<DiscountDto>>(orderLine.Discounts),
                        DeductionCode = orderLine.DeductionCode,
                        DeductionPart1 = orderLine.DeductionPart1,
                        DeductionPart2 = orderLine.DeductionPart2,
                        CurrencyCode = currency.Code,
                        CurrencyRate = orderLine.CurrencyRate,
                        CurrencyPrice = orderLine.CurrencyRate.HasValue
                        ? Math.Round(orderLine.Price / orderLine.CurrencyRate.Value, 2)
                        : null,
                        CurrencyTotal = orderLine.CurrencyRate.HasValue
                        ? Math.Round(orderLine.Total / orderLine.CurrencyRate.Value, 2)
                        : null,
                        ReserveDate = orderLine.ReserveDate,
                        ReserveQuantity = orderLine.ReserveQuantity,
                    };

        input.Lines = await AsyncExecuter.ToListAsync(query);
    }

    public virtual async Task<PagedResultDto<OrderDto>> ListAsync(FilteredPagedAndSortedResultRequestDto input)
    {
        if (string.IsNullOrEmpty(input.Sorting)) input.Sorting = nameof(Order.Id);

        var query = await CreateDtoQueryAsync();

        return await query.PageResultAsync(AsyncExecuter, input);
    }

    public virtual async Task<OrderWithDetailsDto> GetAsync(int id)
    {
        var orderDto = await FindAsync(order => order.Id == id);
        if (orderDto == null)
            throw new EntityNotFoundException(typeof(Order), id);

        await FillLineDetailsDtoAsync(orderDto);

        return orderDto;
    }

    public virtual async Task<OrderWithDetailsDto> GetByNumberAsync(string number, OrderType type)
    {
        var orderDto = await FindAsync(order => order.Number == number && order.Type == type);
        if (orderDto == null)
            throw new NumberNotFoundException(typeof(Order), number);

        await FillLineDetailsDtoAsync(orderDto);

        return orderDto;
    }

    [Authorize(SalerPermissions.OrderManagement.Order.Create)]
    public virtual async Task<OrderWithDetailsDto> CreateAsync(OrderCreateDto input)
    {
        Order order = await OrderManager.CreateAsync(
            input.Number,
            input.Type,
            input.Date,
            statu: input.Statu,
            clientCode: input.ClientCode);

        input.MapExtraPropertiesTo(order, MappingPropertyDefinitionChecks.None);

        foreach (var expectedLine in input.Lines)
        {
            var line = await CreateOrderLineAsync(expectedLine);
            order.Lines.Add(line);
        }

        await OrderManager.CalculateTotals(
            order,
            ObjectMapper.Map<IList<DiscountDto>, IList<Discount>>(input.Discounts),
            input.CurrencyCode,
            input.CurrencyRate);

        await OrderRepository.InsertAsync(order, autoSave: true);

        //TODO maybe uses GetAsync method for other props filling properly (return await GetAsync(item.Id);)
        return ObjectMapper.Map<Order, OrderWithDetailsDto>(order);
    }

    [Authorize(SalerPermissions.OrderManagement.Order.Edit)]
    public virtual async Task<OrderWithDetailsDto> UpdateAsync(int id, OrderUpdateDto input)
    {
        Order order = await OrderRepository.GetAsync(id);

        if (order.Number != input.Number)
            await OrderManager.ChangeNumberAsync(order, input.Number);
        order.SetStatu(input.Statu);
        order.Date = input.Date;
        await OrderManager.SetClientAsync(order, input.ClientCode);
        input.MapExtraPropertiesTo(order, MappingPropertyDefinitionChecks.None);

        RemoveLines(order, input.Lines);
        await UpdateLinesAsync(order, input.Lines);
        await AddLinesAsync(order, input.Lines);
        await OrderManager.CalculateTotals(
            order,
            ObjectMapper.Map<IList<DiscountDto>, IList<Discount>>(input.Discounts),
            input.CurrencyCode,
            input.CurrencyRate);

        await OrderRepository.UpdateAsync(order, autoSave: true);

        //TODO maybe uses GetAsync method for other props filling properly (return await GetAsync(item.Id);)
        return ObjectMapper.Map<Order, OrderWithDetailsDto>(order);
    }
    protected void RemoveLines(
        Order order,
        IList<OrderLineUpdateDto> lines)
    {
        var idList = lines
            .Where(line => line.Id != default)
            .Select(line => line.Id)
            .ToList();

        order.Lines.RemoveAll(line => !idList.Contains(line.Id));
    }
    protected async Task UpdateLinesAsync(
        Order order,
        IList<OrderLineUpdateDto> lines)
    {
        foreach (var expectedLine in lines.Where(unit => unit.Id != default))
        {
            var line = order.Lines.SingleOrDefault(order => order.Id == expectedLine.Id)
                ?? throw new EntityNotFoundException(typeof(OrderLine), expectedLine.Id);

            await OrderManager.SetOrderLineProductAsync(
                line,
                expectedLine.Type,
                expectedLine.ProductCode,
                expectedLine.Quantity,
                expectedLine.UnitCode,
                price: expectedLine.Price,
                vatRate: expectedLine.VatRate,
                isVatIncluded: expectedLine.IsVatIncluded,
                total: expectedLine.Total,
                discounts: ObjectMapper.Map<IList<DiscountDto>, IList<Discount>>(expectedLine.Discounts),
                deductionCode: expectedLine.DeductionCode,
                deductionPart1: expectedLine.DeductionPart1,
                deductionPart2: expectedLine.DeductionPart2,
                currencyCode: expectedLine.CurrencyCode,
                currencyRate: expectedLine.CurrencyRate,
                currencyPrice: expectedLine.CurrencyPrice,
                currencyTotal: expectedLine.CurrencyTotal,
                reserveDate: expectedLine.ReserveDate,
                reserveQuantity: expectedLine.ReserveQuantity);

            await SetOrderLineBaseAsync(line, expectedLine);
            //Set update props
        }
    }
    protected async Task AddLinesAsync(
        Order order,
        IList<OrderLineUpdateDto> lines)
    {
        foreach (var expectedLine in lines.Where(unit => unit.Id == default))
        {
            var line = await CreateOrderLineAsync(expectedLine);
            order.Lines.Add(line);
        }
    }

    protected async virtual Task<OrderLine> CreateOrderLineAsync(OrderLineCreateOrUpdateDtoBase expectedLine)
    {
        var orderLine = await OrderManager.CreateOrderLineAsync(
            expectedLine.Type,
            expectedLine.ProductCode,
            expectedLine.Quantity,
            expectedLine.UnitCode,
            price: expectedLine.Price,
            vatRate: expectedLine.VatRate,
            isVatIncluded: expectedLine.IsVatIncluded,
            total: expectedLine.Total,
            discounts: ObjectMapper.Map<IList<DiscountDto>, IList<Discount>>(expectedLine.Discounts),
            deductionCode: expectedLine.DeductionCode,
            deductionPart1: expectedLine.DeductionPart1,
            deductionPart2: expectedLine.DeductionPart2,
            currencyCode: expectedLine.CurrencyCode,
            currencyRate: expectedLine.CurrencyRate,
            currencyPrice: expectedLine.CurrencyPrice,
            currencyTotal: expectedLine.CurrencyTotal,
            reserveDate: expectedLine.ReserveDate,
            reserveQuantity: expectedLine.ReserveQuantity);

        await SetOrderLineBaseAsync(orderLine, expectedLine);
        //Set create props

        return orderLine;
    }

    protected async virtual Task SetOrderLineBaseAsync(
        OrderLine line,
        OrderLineCreateOrUpdateDtoBase expectedLine)
    {
        //line.ChangeReserve(expectedLine.ReserveDate, expectedLine.ReserveQuantity); must check quantity divisibility

        await Task.CompletedTask;
    }

    [Authorize(SalerPermissions.OrderManagement.Order.Delete)]
    public virtual async Task DeleteAsync(int id)
    {
        //TODO Check any dispatch reference
        await OrderRepository.DeleteAsync(id);
    }
}
