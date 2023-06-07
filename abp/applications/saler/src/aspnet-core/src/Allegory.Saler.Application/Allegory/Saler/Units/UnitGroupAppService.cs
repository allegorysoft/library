using Allegory.Saler.Items;
using Allegory.Saler.Orders;
using Allegory.Saler.Permissions;
using Allegory.Saler.Services;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.ObjectExtending;

namespace Allegory.Saler.Units;

[Authorize(SalerPermissions.ProductManagement.UnitGroup.Default)]
public class UnitGroupAppService : SalerAppService, IUnitGroupAppService
{
    protected IUnitGroupRepository UnitGroupRepository { get; }
    protected UnitGroupManager UnitGroupManager { get; }
    protected IItemRepository ItemRepository => LazyServiceProvider.LazyGetRequiredService<IItemRepository>();
    protected IServiceRepository ServiceRepository => LazyServiceProvider.LazyGetRequiredService<IServiceRepository>();
    protected IReadOnlyRepository<OrderLine, int> OrderLineRepository => LazyServiceProvider.LazyGetRequiredService<IReadOnlyRepository<OrderLine, int>>();

    public UnitGroupAppService(
        IUnitGroupRepository unitGroupRepository,
        UnitGroupManager unitGroupManager)
    {
        UnitGroupRepository = unitGroupRepository;
        UnitGroupManager = unitGroupManager;
    }

    public virtual async Task<PagedResultDto<UnitGroupDto>> ListAsync(FilteredPagedAndSortedResultRequestDto input)
    {
        if (string.IsNullOrEmpty(input.Sorting)) input.Sorting = nameof(UnitGroup.Id);

        var query = await UnitGroupRepository.GetQueryableAsync();

        return await query.PageResultAsync<UnitGroup, UnitGroupDto>(AsyncExecuter, ObjectMapper, input);
    }
    public virtual async Task<UnitGroupWithDetailsDto> GetAsync(int id)
    {
        var unitGroup = await UnitGroupRepository.GetAsync(id);
        return ObjectMapper.Map<UnitGroup, UnitGroupWithDetailsDto>(unitGroup);
    }
    public virtual async Task<UnitGroupWithDetailsDto> GetByCodeAsync(string code)
    {
        var unitGroup = await UnitGroupRepository.GetByCodeAsync(code);
        return ObjectMapper.Map<UnitGroup, UnitGroupWithDetailsDto>(unitGroup);
    }

    [Authorize(SalerPermissions.ProductManagement.UnitGroup.Create)]
    public virtual async Task<UnitGroupWithDetailsDto> CreateAsync(UnitGroupCreateDto input)
    {
        List<Unit> units = new List<Unit>();
        foreach (var unitDto in input.Units)
        {
            var unit = new Unit(
                unitDto.Code,
                unitDto.ConvFact1,
                unitDto.ConvFact2,
                unitDto.Divisible,
                name: unitDto.Name,
                mainUnit: unitDto.MainUnit,
                globalUnitCode: unitDto.GlobalUnitCode);

            units.Add(unit);
        }

        UnitGroup unitGroup = await UnitGroupManager.CreateAsync(
            input.Code,
            units,
            name: input.Name);

        input.MapExtraPropertiesTo(unitGroup, MappingPropertyDefinitionChecks.None);

        await UnitGroupRepository.InsertAsync(unitGroup, autoSave: true);

        return ObjectMapper.Map<UnitGroup, UnitGroupWithDetailsDto>(unitGroup);
    }

    [Authorize(SalerPermissions.ProductManagement.UnitGroup.Edit)]
    public virtual async Task<UnitGroupWithDetailsDto> UpdateAsync(int id, UnitGroupUpdateDto input)
    {
        UnitGroup unitGroup = await UnitGroupRepository.GetAsync(id);

        if (unitGroup.Code != input.Code)
            await UnitGroupManager.ChangeCodeAsync(unitGroup, input.Code);
        unitGroup.SetName(input.Name);
        input.MapExtraPropertiesTo(unitGroup, MappingPropertyDefinitionChecks.None);

        List<Unit> units = new List<Unit>();
        foreach (var unitDto in input.Units)
        {
            var unit = new Unit(
                unitDto.Code,
                unitDto.ConvFact1,
                unitDto.ConvFact2,
                unitDto.Divisible,
                name: unitDto.Name,
                mainUnit: unitDto.MainUnit,
                id: unitDto.Id ?? default,
                globalUnitCode: unitDto.GlobalUnitCode);

            units.Add(unit);
        }

        await UnitGroupManager.UpdateUnitsAsync(unitGroup, units);

        await UnitGroupRepository.UpdateAsync(unitGroup, autoSave: true);

        return ObjectMapper.Map<UnitGroup, UnitGroupWithDetailsDto>(unitGroup);
    }

    [Authorize(SalerPermissions.ProductManagement.UnitGroup.Delete)]
    public virtual async Task DeleteAsync(int id)
    {
        await CheckExistingModules(id);
        await UnitGroupRepository.DeleteAsync(id);
    }

    protected virtual async Task CheckExistingModules(int unitGroupId)
    {
        // Check Item/Service
        if (await ItemRepository.AnyAsync(item => item.UnitGroupId == unitGroupId))
            throw new ThereIsTransactionRecordException(typeof(UnitGroup), typeof(Item), isDelete: true);

        if (await ServiceRepository.AnyAsync(service => service.UnitGroupId == unitGroupId))
            throw new ThereIsTransactionRecordException(typeof(UnitGroup), typeof(Service), isDelete: true);

        //!Eğer malzeme/hizmette kullanılmamışsa sipariş/fatura/irsaliye'lerde kullanılma ihtimali yok çünkü malzeme/hizmet birim grubunu güncellerken hareket varmı diye kontrol ediyoruz
        //var unitGroup = await UnitGroupRepository.GetAsync(unitGroupId);
        //var unitIds = unitGroup.Units.Select(unit => unit.Id).ToList();
        //if (await OrderLineRepository.AnyAsync(orderLine => unitIds.Contains(orderLine.UnitId)))
        //      throw new ThereIsTransactionRecordException(typeof(UnitGroup), typeof(Order), isDelete: true);
    }

    public IList<GlobalUnitDto> GetGlobalUnits()
    {
        return ObjectMapper.Map<
            IList<GlobalUnit>,
            IList<GlobalUnitDto>>
            (LazyServiceProvider.LazyGetRequiredService<IList<GlobalUnit>>());
    }
}
