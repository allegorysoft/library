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

namespace Allegory.Saler.Services;

[Authorize(SalerPermissions.ProductManagement.Service.Default)]
public class ServiceAppService : SalerAppService, IServiceAppService
{
    protected IServiceRepository ServiceRepository { get; }
    protected ServiceManager ServiceManager { get; }
    protected IUnitGroupRepository UnitGroupRepository => LazyServiceProvider.LazyGetRequiredService<IUnitGroupRepository>();
    protected IUnitPriceRepository UnitPriceRepository => LazyServiceProvider.LazyGetRequiredService<IUnitPriceRepository>();
    protected IClientRepository ClientRepository => LazyServiceProvider.LazyGetRequiredService<IClientRepository>();
    protected IReadOnlyRepository<OrderLine, int> OrderLineRepository => LazyServiceProvider.LazyGetRequiredService<IReadOnlyRepository<OrderLine, int>>();
    protected IReadOnlyRepository<Unit, int> UnitRepository => LazyServiceProvider.LazyGetRequiredService<IReadOnlyRepository<Unit, int>>();

    public ServiceAppService(
        IServiceRepository serviceRepository,
        ServiceManager serviceManager)
    {
        ServiceRepository = serviceRepository;
        ServiceManager = serviceManager;
    }

    protected virtual async Task<IQueryable<ServiceDto>> CreateDtoQueryAsync()
    {
        var query = from service in await ServiceRepository.GetQueryableAsync()
                    join unit in await UnitRepository.GetQueryableAsync() on
                    new { UnitGroupId = service.UnitGroupId, MainUnit = true } equals
                    new { UnitGroupId = unit.UnitGroupId, MainUnit = unit.MainUnit }
                    select new ServiceDto
                    {
                        Id = service.Id,
                        Code = service.Code,
                        Name = service.Name,
                        //ExtraProperties = item.ExtraProperties, => protected set
                        CreatorId = service.CreatorId,
                        CreationTime = service.CreationTime,
                        LastModifierId = service.LastModifierId,
                        LastModificationTime = service.LastModificationTime,
                        MainUnitCode = unit.Code,
                    };

        return query;
    }
    protected virtual async Task<IQueryable<ServiceWithDetailsDto>> CreateDetailsDtoQueryAsync()
    {
        var query = from service in await ServiceRepository.GetQueryableAsync()
                    join unitGroup in await UnitGroupRepository.GetQueryableAsync() on service.UnitGroupId equals unitGroup.Id
                    select new ServiceWithDetailsDto
                    {
                        Id = service.Id,
                        Code = service.Code,
                        Name = service.Name,
                        //ExtraProperties = item.ExtraProperties, => protected set
                        CreatorId = service.CreatorId,
                        CreationTime = service.CreationTime,
                        LastModifierId = service.LastModifierId,
                        LastModificationTime = service.LastModificationTime,
                        UnitGroupCode = unitGroup.Code,
                        DeductionCode = service.DeductionCode,
                        SalesDeductionPart1 = service.SalesDeductionPart1,
                        SalesDeductionPart2 = service.SalesDeductionPart2,
                        PurchaseDeductionPart1 = service.PurchaseDeductionPart1,
                        PurchaseDeductionPart2 = service.PurchaseDeductionPart2,
                        PurchaseVatRate = service.PurchaseVatRate,
                        SalesVatRate = service.SalesVatRate,
                    };

        return query;
    }
    protected virtual async Task<ServiceWithDetailsDto> FindAsync(Expression<Func<ServiceWithDetailsDto, bool>> predicate)
    {
        var query = await CreateDetailsDtoQueryAsync();
        return await AsyncExecuter.FirstOrDefaultAsync(query.Where(predicate));
    }

    public virtual async Task<PagedResultDto<ServiceDto>> ListAsync(FilteredPagedAndSortedResultRequestDto input)
    {
        if (string.IsNullOrEmpty(input.Sorting)) input.Sorting = nameof(Service.Id);

        var query = await CreateDtoQueryAsync();

        return await query.PageResultAsync(AsyncExecuter, input);
    }

    public virtual async Task<PagedResultDto<ServiceLookupDto>> ListServiceLookupAsync(GetServiceLookupListDto input)
    {
        if (string.IsNullOrEmpty(input.Sorting)) input.Sorting = nameof(Service.Id);

        var unitPriceQuery = await UnitPriceRepository.GetQueryableAsync();

        int? clientId = null;
        if (!input.ClientCode.IsNullOrWhiteSpace())
            clientId = (await ClientRepository.GetByCodeAsync(input.ClientCode, false)).Id;

        var query = from service in await ServiceRepository.GetQueryableAsync()
                    
                    join unit in await UnitRepository.GetQueryableAsync() on
                    new { UnitGroupId = service.UnitGroupId, MainUnit = true } equals
                    new { UnitGroupId = unit.UnitGroupId, MainUnit = unit.MainUnit }

                    from unitPrice in unitPriceQuery
                    .Where(u => u.ProductId == service.Id
                             && u.Type == UnitPriceType.Service
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

                    select new ServiceLookupDto
                    {
                        Id = service.Id,
                        Code = service.Code,
                        Name = service.Name,
                        //ExtraProperties = item.ExtraProperties, => protected set
                        CreatorId = service.CreatorId,
                        CreationTime = service.CreationTime,
                        LastModifierId = service.LastModifierId,
                        LastModificationTime = service.LastModificationTime,
                        MainUnitCode = unit.Code,
                        Price = unitPrice.IsVatIncluded
                        ? unitPrice.Price
                        / (1 + (decimal)(input.IsSales
                                ? service.SalesVatRate
                                : service.PurchaseVatRate)
                                / 100)
                        : unitPrice.Price,
                        VatIncludedPrice = unitPrice.IsVatIncluded
                        ? unitPrice.Price
                        : unitPrice.Price
                        * (1 + (decimal)(input.IsSales
                                ? service.SalesVatRate
                                : service.PurchaseVatRate)
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

        return result;
    }

    public virtual async Task<ServiceWithDetailsDto> GetAsync(int id)
    {
        var serviceDto = await FindAsync(service => service.Id == id);
        if (serviceDto == null)
            throw new EntityNotFoundException(typeof(Service), id);

        return serviceDto;
    }

    public virtual async Task<ServiceWithDetailsDto> GetByCodeAsync(string code)
    {
        var serviceDto = await FindAsync(item => item.Code == code);
        if (serviceDto == null)
            throw new CodeNotFoundException(typeof(Service), code);

        return serviceDto;
    }

    [Authorize(SalerPermissions.ProductManagement.Service.Create)]
    public virtual async Task<ServiceWithDetailsDto> CreateAsync(ServiceCreateDto input)
    {
        Service service = await ServiceManager.CreateAsync(
            input.Code,
            input.UnitGroupCode);

        await SetServiceBaseAsync(service, input);

        await ServiceRepository.InsertAsync(service, autoSave: true);

        //To do may be uses GetAsync method for other props filling properly (return await GetAsync(item.Id);)
        return ObjectMapper.Map<Service, ServiceWithDetailsDto>(service);
    }

    [Authorize(SalerPermissions.ProductManagement.Service.Edit)]
    public virtual async Task<ServiceWithDetailsDto> UpdateAsync(int id, ServiceUpdateDto input)
    {
        Service service = await ServiceRepository.GetAsync(id);

        if (service.Code != input.Code)
            await ServiceManager.ChangeCodeAsync(service, input.Code);

        await ServiceManager.ChangeUnitGroupAsync(service, input.UnitGroupCode);

        await SetServiceBaseAsync(service, input);

        await ServiceRepository.UpdateAsync(service);

        //To do may be uses GetAsync method for other props filling properly (return await GetAsync(item.Id);)
        return ObjectMapper.Map<Service, ServiceWithDetailsDto>(service);
    }

    protected virtual async Task SetServiceBaseAsync(
        Service service,
        ServiceCreateOrUpdateDtoBase input)
    {
        service.SetName(input.Name);
        input.MapExtraPropertiesTo(service, MappingPropertyDefinitionChecks.None);
        service.SetSalesVatRate(input.SalesVatRate);
        service.SetPurchaseVatRate(input.PurchaseVatRate);

        ServiceManager.SetDeduction(
            service,
            input.DeductionCode,
            input.SalesDeductionPart1,
            input.SalesDeductionPart2,
            input.PurchaseDeductionPart1,
            input.PurchaseDeductionPart2);

        await Task.CompletedTask;
    }

    [Authorize(SalerPermissions.ProductManagement.Service.Delete)]
    public virtual async Task DeleteAsync(int id)
    {
        if (await OrderLineRepository.AnyAsync(orderLine => orderLine.Type == OrderLineType.Service && orderLine.ProductId == id))
            throw new ThereIsTransactionRecordException(typeof(Service), typeof(Order), isDelete: true);

        await UnitPriceRepository.DeleteAsync(
            x => x.Type == UnitPriceType.Service && x.ProductId == id);

        await ServiceRepository.DeleteAsync(id);
    }
}
