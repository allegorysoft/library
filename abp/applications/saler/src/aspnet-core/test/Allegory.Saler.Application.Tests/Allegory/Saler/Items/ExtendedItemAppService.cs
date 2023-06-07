using Allegory.Saler.UnitPrices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;

namespace Allegory.Saler.Items;

[ExposeServices(typeof(IItemAppService))]
[Dependency(ReplaceServices = true)]
public class ExtendedItemAppService : ItemAppService
{
    public ExtendedItemAppService(
        IItemRepository itemRepository,
        ItemManager itemManager)
        : base(itemRepository, itemManager)
    {

    }

    protected async override Task<IQueryable<ItemDto>> CreateDtoQueryAsync()
    {
        var query = from item in await ItemRepository.GetQueryableAsync()
                    join unit in await UnitRepository.GetQueryableAsync() on
                    new { UnitGroupId = item.UnitGroupId, MainUnit = true } equals
                    new { UnitGroupId = unit.UnitGroupId, MainUnit = unit.MainUnit }

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
                    };

        return query;
    }

    public async override Task<PagedResultDto<ItemDto>> ListAsync(FilteredPagedAndSortedResultRequestDto input)
    {
        var list = await base.ListAsync(input);

        await FillStockAsync(list.Items);

        return list;
    }

    protected async Task FillStockAsync(IReadOnlyList<ItemDto> items)
    {
        var queryable = await ItemStockTransactionRepository.GetQueryableAsync();

        var query = queryable.GroupBy(u => u.ItemId)
            .Where(u => items.Select(x => x.Id).Contains(u.Key))
            .Select(u => new
            {
                ItemId = u.Key,
                Stock = u.Sum(f => (double?)f.Quantity
                               * (f.Statu == ItemStockTransactionStatu.Approved
                                  ? f.IsOutput ? -1 : 1
                                  : 0)),
                ReservedStock = u.Sum(f => (double?)f.Quantity
                 * ((f.Statu == ItemStockTransactionStatu.Approved
                     || f.Statu == ItemStockTransactionStatu.Reserved)
                    ? f.IsOutput ? -1 : 1
                    : 0))
            });

        var stockInfo = await AsyncExecuter.ToListAsync(query);

        items.Select(x =>
        {
            x.Stock = (decimal?)stockInfo.FirstOrDefault(y => x.Id == y.ItemId)?.Stock;
            x.ReservedStock = (decimal?)stockInfo.FirstOrDefault(y => x.Id == y.ItemId)?.ReservedStock;
            return x;
        }).ToList();
    }

    public async override Task<PagedResultDto<ItemLookupDto>> ListItemLookupAsync(GetItemLookupListDto input)
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
                                / 100)
                    };

        var result = await query.PageResultAsync(AsyncExecuter, input);

        result.Items.ToList().ForEach(i =>
        {
            if (i.Price.HasValue)
                i.Price = Math.Round(i.Price.Value, 5);
            if (i.VatIncludedPrice.HasValue)
                i.VatIncludedPrice = Math.Round(i.VatIncludedPrice.Value, 5);
        });//can't do with linq cause sql data type is float

        await FillStockAsync(result.Items);

        return result;
    }
}
