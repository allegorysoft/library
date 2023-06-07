using Allegory.Saler.Clients;
using Allegory.Saler.Currencies;
using Allegory.Saler.Items;
using Allegory.Saler.Permissions;
using Allegory.Saler.Services;
using Allegory.Saler.Units;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;

namespace Allegory.Saler.UnitPrices;

[Authorize(SalerPermissions.ProductManagement.UnitPrice.Default)]
public class UnitPriceAppService : SalerAppService, IUnitPriceAppService
{
    protected IUnitPriceRepository UnitPriceRepository { get; }
    protected UnitPriceManager UnitPriceManager { get; }
    protected ICurrencyRepository CurrencyRepository => LazyServiceProvider.LazyGetRequiredService<ICurrencyRepository>();
    protected IServiceRepository ServiceRepository => LazyServiceProvider.LazyGetRequiredService<IServiceRepository>();
    protected IItemRepository ItemRepository => LazyServiceProvider.LazyGetRequiredService<IItemRepository>();
    protected IClientRepository ClientRepository => LazyServiceProvider.LazyGetRequiredService<IClientRepository>();
    protected IReadOnlyRepository<Unit, int> UnitRepository => LazyServiceProvider.LazyGetRequiredService<IReadOnlyRepository<Unit, int>>();

    public UnitPriceAppService(
        IUnitPriceRepository unitPriceRepository,
        UnitPriceManager unitPriceManager)
    {
        UnitPriceRepository = unitPriceRepository;
        UnitPriceManager = unitPriceManager;
    }

    protected async Task<IQueryable<UnitPriceDto>> CreateDtoQueryAsync()
    {
        var query = from unitPrice in await UnitPriceRepository.GetQueryableAsync()

                    join unit in await UnitRepository.GetQueryableAsync()
                    on unitPrice.UnitId equals unit.Id

                    join currency in await CurrencyRepository.GetQueryableAsync()
                    on unitPrice.CurrencyId equals currency.Id into currencies
                    from currency in currencies.DefaultIfEmpty()

                    join client in await ClientRepository.GetQueryableAsync()
                    on unitPrice.ClientId equals client.Id into clients
                    from client in clients.DefaultIfEmpty()

                    join item in await ItemRepository.GetQueryableAsync()
                    on new
                    {
                        ProductId = unitPrice.ProductId,
                        Type = unitPrice.Type
                    }
                    equals new
                    {
                        ProductId = item.Id,
                        Type = UnitPriceType.Item
                    } into items
                    from item in items.DefaultIfEmpty()

                    join service in await ServiceRepository.GetQueryableAsync()
                    on new
                    {
                        ProductId = unitPrice.ProductId,
                        Type = unitPrice.Type
                    }
                    equals new
                    {
                        ProductId = service.Id,
                        Type = UnitPriceType.Service
                    } into services
                    from service in services.DefaultIfEmpty()

                    select new UnitPriceDto
                    {
                        Id = unitPrice.Id,
                        Code = unitPrice.Code,
                        Type = unitPrice.Type,
                        IsVatIncluded = unitPrice.IsVatIncluded,
                        PurchasePrice = unitPrice.PurchasePrice,
                        SalesPrice = unitPrice.SalesPrice,
                        UnitCode = unit.Code,
                        ProductCode = unitPrice.Type == UnitPriceType.Item
                                     ? item.Code : service.Code,
                        ProductName = unitPrice.Type == UnitPriceType.Item
                                     ? item.Name : service.Name,
                        //ExtraProperties = item.ExtraProperties, => protected set
                        CreatorId = unitPrice.CreatorId,
                        CreationTime = unitPrice.CreationTime,
                        LastModifierId = unitPrice.LastModifierId,
                        LastModificationTime = unitPrice.LastModificationTime,
                        CurrencyCode = currency.Code,
                        ClientCode = client.Code
                    };

        return query;
    }
    protected async Task<IQueryable<UnitPriceWithDetailsDto>> CreateDetailsDtoQueryAsync()
    {
        var query = from unitPrice in await UnitPriceRepository.GetQueryableAsync()

                    join unit in await UnitRepository.GetQueryableAsync()
                    on unitPrice.UnitId equals unit.Id

                    join currency in await CurrencyRepository.GetQueryableAsync()
                    on unitPrice.CurrencyId equals currency.Id into currencies
                    from currency in currencies.DefaultIfEmpty()

                    join client in await ClientRepository.GetQueryableAsync()
                    on unitPrice.ClientId equals client.Id into clients
                    from client in clients.DefaultIfEmpty()

                    join item in await ItemRepository.GetQueryableAsync()
                    on new
                    {
                        ProductId = unitPrice.ProductId,
                        Type = unitPrice.Type
                    }
                    equals new
                    {
                        ProductId = item.Id,
                        Type = UnitPriceType.Item
                    } into items
                    from item in items.DefaultIfEmpty()

                    join service in await ServiceRepository.GetQueryableAsync()
                    on new
                    {
                        ProductId = unitPrice.ProductId,
                        Type = unitPrice.Type
                    }
                    equals new
                    {
                        ProductId = service.Id,
                        Type = UnitPriceType.Service
                    } into services
                    from service in services.DefaultIfEmpty()

                    select new UnitPriceWithDetailsDto
                    {
                        Id = unitPrice.Id,
                        Code = unitPrice.Code,
                        Type = unitPrice.Type,
                        IsVatIncluded = unitPrice.IsVatIncluded,
                        PurchasePrice = unitPrice.PurchasePrice,
                        SalesPrice = unitPrice.SalesPrice,
                        UnitCode = unit.Code,
                        ProductCode = unitPrice.Type == UnitPriceType.Item
                                     ? item.Code : service.Code,
                        ProductName = unitPrice.Type == UnitPriceType.Item
                                     ? item.Name : service.Name,
                        //ExtraProperties = item.ExtraProperties, => protected set
                        CreatorId = unitPrice.CreatorId,
                        CreationTime = unitPrice.CreationTime,
                        LastModifierId = unitPrice.LastModifierId,
                        LastModificationTime = unitPrice.LastModificationTime,
                        CurrencyCode = currency.Code,
                        ClientCode = client.Code,
                        BeginDate = unitPrice.BeginDate,
                        EndDate = unitPrice.EndDate
                    };

        return query;
    }
    protected async Task<UnitPriceWithDetailsDto> FindAsync(Expression<Func<UnitPriceWithDetailsDto, bool>> predicate)
    {
        var query = await CreateDetailsDtoQueryAsync();
        return await AsyncExecuter.FirstOrDefaultAsync(query.Where(predicate));
    }

    public async Task<PagedResultDto<UnitPriceDto>> ListAsync(FilteredPagedAndSortedResultRequestDto input)
    {
        if (string.IsNullOrEmpty(input.Sorting)) input.Sorting = nameof(UnitPrice.Id);

        var query = await CreateDtoQueryAsync();

        return await query.PageResultAsync(AsyncExecuter, input);
    }

    public virtual async Task<UnitPriceWithDetailsDto> GetAsync(int id)
    {
        var unitPriceDto = await FindAsync(order => order.Id == id);
        if (unitPriceDto == null)
            throw new EntityNotFoundException(typeof(UnitPrice), id);

        return unitPriceDto;
    }

    public virtual async Task<UnitPriceWithDetailsDto> GetByCodeAsync(
        string code,
        UnitPriceType type)
    {
        var unitPriceDto = await FindAsync(unitPrice => unitPrice.Code == code
                                                 && unitPrice.Type == type);
        if (unitPriceDto == null)
            throw new CodeNotFoundException(typeof(UnitPrice), code);

        return unitPriceDto;
    }

    [Authorize(SalerPermissions.ProductManagement.UnitPrice.Create)]
    public async Task<UnitPriceWithDetailsDto> CreateAsync(UnitPriceCreateDto input)
    {
        UnitPrice unitPrice = await UnitPriceManager.CreateAsync(
            input.Code,
            input.Type,
            input.ProductCode,
            input.UnitCode,
            input.PurchasePrice,
            input.SalesPrice,
            input.BeginDate,
            input.EndDate,
            input.CurrencyCode,
            input.ClientCode);

        unitPrice.IsVatIncluded = input.IsVatIncluded;

        await UnitPriceRepository.InsertAsync(unitPrice, autoSave: true);

        //To do may be uses GetAsync method for other props filling properly (return await GetAsync(item.Id);)
        return ObjectMapper.Map<UnitPrice, UnitPriceWithDetailsDto>(unitPrice);
    }

    [Authorize(SalerPermissions.ProductManagement.UnitPrice.Edit)]
    public async Task<UnitPriceWithDetailsDto> UpdateAsync(
        int id,
        UnitPriceUpdateDto input)
    {
        var unitPrice = await UnitPriceRepository.GetAsync(id);

        if (unitPrice.Code != input.Code)
            await UnitPriceManager.ChangeCodeAsync(unitPrice, input.Code);

        await UnitPriceManager.SetProductAsync(
            unitPrice,
            input.ProductCode,
            input.UnitCode);

        unitPrice.SetDates(input.BeginDate, input.EndDate);
        unitPrice.SetPrice(input.PurchasePrice, input.SalesPrice);
        unitPrice.IsVatIncluded = input.IsVatIncluded;
        await UnitPriceManager.SetCurrencyAsync(unitPrice, input.CurrencyCode);
        await UnitPriceManager.SetClientAsync(unitPrice, input.ClientCode);

        await UnitPriceRepository.UpdateAsync(unitPrice);

        //To do may be uses GetAsync method for other props filling properly (return await GetAsync(item.Id);)
        return ObjectMapper.Map<UnitPrice, UnitPriceWithDetailsDto>(unitPrice);
    }

    [Authorize(SalerPermissions.ProductManagement.UnitPrice.Delete)]
    public async Task DeleteAsync(int id)
    {
        await UnitPriceRepository.DeleteAsync(id);
    }

    public async Task<decimal> GetPriceAsync(
        string productCode,
        UnitPriceType type,
        string unitCode,
        DateTime date,
        bool isSales,
        byte? vatRate = default,
        string currencyCode = default,
        string clientCode = default)
    {
        return await UnitPriceManager.GetPriceAsync(
            productCode,
            type,
            unitCode,
            date,
            isSales,
            vatRate,
            currencyCode,
            clientCode);
    }
}
