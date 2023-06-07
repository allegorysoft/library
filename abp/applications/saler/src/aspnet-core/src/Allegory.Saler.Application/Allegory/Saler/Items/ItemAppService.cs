using Allegory.Saler.Clients;
using Allegory.Saler.Orders;
using Allegory.Saler.Permissions;
using Allegory.Saler.UnitPrices;
using Allegory.Saler.Units;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.ObjectExtending;

namespace Allegory.Saler.Items;

[Authorize(SalerPermissions.ProductManagement.Item.Default)]
public class ItemAppService : SalerAppService, IItemAppService
{
    protected IItemRepository ItemRepository { get; }
    protected ItemManager ItemManager { get; }
    protected IUnitGroupRepository UnitGroupRepository => LazyServiceProvider.LazyGetRequiredService<IUnitGroupRepository>();
    protected IUnitPriceRepository UnitPriceRepository => LazyServiceProvider.LazyGetRequiredService<IUnitPriceRepository>();
    protected IClientRepository ClientRepository => LazyServiceProvider.LazyGetRequiredService<IClientRepository>();
    protected IReadOnlyRepository<OrderLine, int> OrderLineRepository => LazyServiceProvider.LazyGetRequiredService<IReadOnlyRepository<OrderLine, int>>();
    protected IReadOnlyRepository<Unit, int> UnitRepository => LazyServiceProvider.LazyGetRequiredService<IReadOnlyRepository<Unit, int>>();
    protected IItemStockTransactionRepository ItemStockTransactionRepository => LazyServiceProvider.LazyGetRequiredService<IItemStockTransactionRepository>();

    public ItemAppService(
        IItemRepository itemRepository,
        ItemManager itemManager)
    {
        ItemRepository = itemRepository;
        ItemManager = itemManager;
    }

    protected virtual async Task<IQueryable<ItemDto>> CreateDtoQueryAsync()
    {
        var stockTransactionQuery = await ItemStockTransactionRepository.GetQueryableAsync();
        var query = from item in await ItemRepository.GetQueryableAsync()
                    join unit in await UnitRepository.GetQueryableAsync() on
                    new { UnitGroupId = item.UnitGroupId, MainUnit = true } equals
                    new { UnitGroupId = unit.UnitGroupId, MainUnit = unit.MainUnit }

                    from stockTransaction in stockTransactionQuery
                    .Where(u => u.ItemId == item.Id)
                    .GroupBy(u => u.ItemId)
                    .Select(u => new
                    {
                        Stock = u.Sum(f => f.Quantity
                        * (f.Statu == ItemStockTransactionStatu.Approved
                           ? f.IsOutput ? -1 : 1
                           : 0)),
                        ReservedStock = u.Sum(f => f.Quantity
                         * ((f.Statu == ItemStockTransactionStatu.Approved
                             || f.Statu == ItemStockTransactionStatu.Reserved)
                            ? f.IsOutput ? -1 : 1
                            : 0))
                    })
                    .DefaultIfEmpty()

                    select new ItemDto
                    {
                        Id = item.Id,
                        Code = item.Code,
                        Name = item.Name,
                        Type = item.Type,
                        //ExtraProperties = item.ExtraProperties, => protected set
                        CreatorId = item.CreatorId,
                        CreationTime = item.CreationTime,
                        LastModifierId = item.LastModifierId,
                        LastModificationTime = item.LastModificationTime,
                        MainUnitCode = unit.Code,
                        Stock = stockTransaction.Stock,
                        ReservedStock = stockTransaction.ReservedStock
                    };

        return query;
    }
    protected virtual async Task<IQueryable<ItemWithDetailsDto>> CreateDetailsDtoQueryAsync()
    {
        var query = from item in await ItemRepository.GetQueryableAsync()
                    join unitGroup in await UnitGroupRepository.GetQueryableAsync() on item.UnitGroupId equals unitGroup.Id
                    select new ItemWithDetailsDto
                    {
                        Id = item.Id,
                        Code = item.Code,
                        Name = item.Name,
                        Type = item.Type,
                        //ExtraProperties = item.ExtraProperties, => protected set
                        CreatorId = item.CreatorId,
                        CreationTime = item.CreationTime,
                        LastModifierId = item.LastModifierId,
                        LastModificationTime = item.LastModificationTime,
                        UnitGroupCode = unitGroup.Code,
                        DeductionCode = item.DeductionCode,
                        SalesDeductionPart1 = item.SalesDeductionPart1,
                        SalesDeductionPart2 = item.SalesDeductionPart2,
                        PurchaseDeductionPart1 = item.PurchaseDeductionPart1,
                        PurchaseDeductionPart2 = item.PurchaseDeductionPart2,
                        SalesVatRate = item.SalesVatRate,
                        PurchaseVatRate = item.PurchaseVatRate
                    };

        return query;
    }
    protected virtual async Task<ItemWithDetailsDto> FindAsync(Expression<Func<ItemWithDetailsDto, bool>> predicate)
    {
        var query = await CreateDetailsDtoQueryAsync();
        return await AsyncExecuter.FirstOrDefaultAsync(query.Where(predicate));
    }

    public virtual async Task<PagedResultDto<ItemDto>> ListAsync(FilteredPagedAndSortedResultRequestDto input)
    {
        if (string.IsNullOrEmpty(input.Sorting)) input.Sorting = nameof(Item.Id);

        var query = await CreateDtoQueryAsync();

        return await query.PageResultAsync(AsyncExecuter, input);
    }

    public virtual async Task<PagedResultDto<ItemLookupDto>> ListItemLookupAsync(GetItemLookupListDto input)
    {
        if (string.IsNullOrEmpty(input.Sorting)) input.Sorting = nameof(Item.Id);

        var unitPriceQuery = await UnitPriceRepository.GetQueryableAsync();
        var stockTransactionQuery = await ItemStockTransactionRepository.GetQueryableAsync();

        int? clientId = null;
        if (!input.ClientCode.IsNullOrWhiteSpace())
            clientId = (await ClientRepository.GetByCodeAsync(input.ClientCode, false)).Id;

        var query = from item in await ItemRepository.GetQueryableAsync()

                    join unit in await UnitRepository.GetQueryableAsync() on
                    new { UnitGroupId = item.UnitGroupId, MainUnit = true } equals
                    new { UnitGroupId = unit.UnitGroupId, MainUnit = unit.MainUnit }

                    from stockTransaction in stockTransactionQuery
                    .Where(u => u.ItemId == item.Id)
                    .GroupBy(u => u.ItemId)
                    .Select(u => new
                    {
                        Stock = u.Sum(f => f.Quantity
                        * (f.Statu == ItemStockTransactionStatu.Approved
                           ? f.IsOutput ? -1 : 1
                           : 0)),
                        ReservedStock = u.Sum(f => f.Quantity
                         * ((f.Statu == ItemStockTransactionStatu.Approved
                             || f.Statu == ItemStockTransactionStatu.Reserved)
                            ? f.IsOutput ? -1 : 1
                            : 0))
                    })
                    .DefaultIfEmpty()

                    from unitPrice in unitPriceQuery
                    .Where(u => u.ProductId == item.Id
                             && u.Type == UnitPriceType.Item
                             && input.Date >= u.BeginDate
                             && input.Date <= u.EndDate
                             && u.UnitId == unit.Id
                             && (u.ClientId == null || u.ClientId == clientId)
                             && u.CurrencyId == null)
                    .OrderByDescending(u => u.ClientId).ThenBy(c => c.Id)
                    .Select(u => new
                    {
                        Price = input.IsSales ? u.SalesPrice : u.PurchasePrice,
                        IsVatIncluded = u.IsVatIncluded
                    })
                    .Take(1)
                    .DefaultIfEmpty()

                    select new ItemLookupDto
                    {
                        Id = item.Id,
                        Code = item.Code,
                        Name = item.Name,
                        Type = item.Type,
                        //ExtraProperties = item.ExtraProperties, => protected set
                        CreatorId = item.CreatorId,
                        CreationTime = item.CreationTime,
                        LastModifierId = item.LastModifierId,
                        LastModificationTime = item.LastModificationTime,
                        MainUnitCode = unit.Code,
                        Price = unitPrice.IsVatIncluded
                        ? unitPrice.Price
                        / (1 + (decimal)(input.IsSales
                                ? item.SalesVatRate
                                : item.PurchaseVatRate)
                                / 100)
                        : unitPrice.Price,
                        VatIncludedPrice = unitPrice.IsVatIncluded
                        ? unitPrice.Price
                        : unitPrice.Price
                        * (1 + (decimal)(input.IsSales
                                ? item.SalesVatRate
                                : item.PurchaseVatRate)
                                / 100),
                        Stock = stockTransaction.Stock,
                        ReservedStock = stockTransaction.ReservedStock
                    };

        var result = await query.PageResultAsync(AsyncExecuter, input);

        result.Items.ToList().ForEach(i =>
        {
            if (i.Price.HasValue)
                i.Price = Math.Round(i.Price.Value, 5);
            if (i.VatIncludedPrice.HasValue)
                i.VatIncludedPrice = Math.Round(i.VatIncludedPrice.Value, 5);
        });//can't do with linq cause sql data type is float

        return result;
    }

    public virtual async Task<ItemWithDetailsDto> GetAsync(int id)
    {
        var itemDto = await FindAsync(item => item.Id == id);
        if (itemDto == null)
            throw new EntityNotFoundException(typeof(Item), id);

        return itemDto;
    }

    public virtual async Task<ItemWithDetailsDto> GetByCodeAsync(string code)
    {
        var itemDto = await FindAsync(item => item.Code == code);
        if (itemDto == null)
            throw new CodeNotFoundException(typeof(Item), code);

        return itemDto;
    }

    [Authorize(SalerPermissions.ProductManagement.Item.Create)]
    public virtual async Task<ItemWithDetailsDto> CreateAsync(ItemCreateDto input)
    {
        Item item = await ItemManager.CreateAsync(
            input.Code,
            input.Type,
            input.UnitGroupCode);

        await SetItemBaseAsync(item, input);

        await ItemRepository.InsertAsync(item, autoSave: true);

        //To do may be uses GetAsync method for other props filling properly (return await GetAsync(item.Id);)
        return ObjectMapper.Map<Item, ItemWithDetailsDto>(item);
    }

    [Authorize(SalerPermissions.ProductManagement.Item.Edit)]
    public virtual async Task<ItemWithDetailsDto> UpdateAsync(int id, ItemUpdateDto input)
    {
        Item item = await ItemRepository.GetAsync(id);

        if (item.Code != input.Code)
            await ItemManager.ChangeCodeAsync(item, input.Code);

        await ItemManager.ChangeUnitGroupAsync(item, input.UnitGroupCode);

        await SetItemBaseAsync(item, input);

        await ItemRepository.UpdateAsync(item);

        //To do may be uses GetAsync method for other props filling properly (return await GetAsync(item.Id);)
        return ObjectMapper.Map<Item, ItemWithDetailsDto>(item);
    }

    protected virtual async Task SetItemBaseAsync(
        Item item,
        ItemCreateOrUpdateDtoBase input)
    {
        item.SetName(input.Name);
        input.MapExtraPropertiesTo(item, MappingPropertyDefinitionChecks.None);
        item.SetSalesVatRate(input.SalesVatRate);
        item.SetPurchaseVatRate(input.PurchaseVatRate);

        ItemManager.SetDeduction(
            item,
            input.DeductionCode,
            input.SalesDeductionPart1,
            input.SalesDeductionPart2,
            input.PurchaseDeductionPart1,
            input.PurchaseDeductionPart2);

        await Task.CompletedTask;
    }

    [Authorize(SalerPermissions.ProductManagement.Item.Delete)]
    public virtual async Task DeleteAsync(int id)
    {
        if (await OrderLineRepository.AnyAsync(orderLine => orderLine.Type == OrderLineType.Item && orderLine.ProductId == id))
            throw new ThereIsTransactionRecordException(typeof(Item), typeof(Order), isDelete: true);

        await UnitPriceRepository.DeleteAsync(
            x => x.Type == UnitPriceType.Item && x.ProductId == id);

        await ItemRepository.DeleteAsync(id);
    }

}
