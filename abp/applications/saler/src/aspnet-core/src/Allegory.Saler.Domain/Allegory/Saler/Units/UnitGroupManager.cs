using Allegory.Saler.Orders;
using Allegory.Saler.UnitPrices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;

namespace Allegory.Saler.Units;

public class UnitGroupManager : SalerDomainService
{
    protected IReadOnlyRepository<OrderLine, int> OrderLineRepository => LazyServiceProvider.LazyGetRequiredService<IReadOnlyRepository<OrderLine, int>>();
    protected IUnitGroupRepository UnitGroupRepository { get; }
    protected IUnitPriceRepository UnitPriceRepository => LazyServiceProvider.LazyGetRequiredService<IUnitPriceRepository>();

    public UnitGroupManager(IUnitGroupRepository unitGroupRepository)
    {
        UnitGroupRepository = unitGroupRepository;
    }

    public async Task<UnitGroup> CreateAsync(
        string code,
        IList<Unit> units,
        string name = default)
    {
        await CheckUnitGroupExistsAsync(code);

        CheckUnitValidation(units);

        var unitGroup = new UnitGroup(
            code,
            name: name);

        foreach (var unit in units)
            AddUnit(unitGroup, unit);

        return unitGroup;
    }

    public async Task CheckUnitGroupExistsAsync(
        string code,
        int? unitGroupId = default)
    {
        Expression<Func<UnitGroup, bool>> expression = unitGroup => unitGroup.Code == code;

        if (unitGroupId != default)
            expression = expression.And(unitGroup => unitGroup.Id != unitGroupId);

        var unitGroupExists = await UnitGroupRepository.AnyAsync(expression);

        if (unitGroupExists)
            throw new CodeAlreadyExistsException(typeof(UnitGroup), code);
    }

    #region Unit validation
    protected void CheckUnitValidation(IList<Unit> units)
    {
        CheckUnitCount(units);
        CheckMainUnit(units);
    }
    protected void CheckUnitCount(IList<Unit> units)
    {
        if ((units?.Count ?? 0) == 0)
            throw new UnitGroupMustAtLeastOneUnitException();
    }
    protected void CheckMainUnit(IList<Unit> units)
    {
        if (units.Count(unit => unit.MainUnit) != 1)
            throw new UnitGroupMustOneMainUnitException();
    }
    #endregion

    #region Update units
    public async Task UpdateUnitsAsync(
        UnitGroup unitGroup,
        IList<Unit> units)
    {
        CheckUnitValidation(units);
        await RemoveUnitAsync(unitGroup, units);
        await UpdateUnitAsync(unitGroup, units);
        AddUnit(unitGroup, units);
    }

    protected async Task RemoveUnitAsync(
        UnitGroup unitGroup,
        IList<Unit> units)
    {
        var idList = units
            .Where(unit => unit.Id != default)
            .Select(unit => unit.Id)
            .ToList();

        var deletedUnitIds = unitGroup
            .Units
            .Where(unit => !idList.Contains(unit.Id))
            .Select(unit => unit.Id)
            .ToArray();

        await CheckUnitRelations(true, deletedUnitIds);

        unitGroup.UnitLines.RemoveAll(unit => deletedUnitIds.Contains(unit.Id));
    }

    protected async Task CheckUnitRelations(
        bool isDelete = false,
        params int[] unitIds)
    {
        if (unitIds.Length > 0)
        {
            if (await OrderLineRepository.AnyAsync(orderLine => unitIds.Contains(orderLine.UnitId)))
                throw new ThereIsTransactionRecordException(typeof(Unit), typeof(Order), isDelete: isDelete);
        }
    }

    protected async Task UpdateUnitAsync(
        UnitGroup unitGroup,
        IList<Unit> units)
    {
        foreach (var expectedUnit in units.Where(unit => unit.Id != default))
        {
            var unit = unitGroup.Units.SingleOrDefault(unit => unit.Id == expectedUnit.Id)
                       ?? throw new UnitDoesnotBelongUnitGroupException(expectedUnit.Code, unitGroup.Code);

            unitGroup.CheckUnitCodeExists(expectedUnit.Code, expectedUnit.Id);
            unit.SetCode(expectedUnit.Code);
            unit.SetName(expectedUnit.Name);
            unit.SetGlobalUnitCode(expectedUnit.GlobalUnitCode);
            unit.MainUnit = expectedUnit.MainUnit;

            if (!expectedUnit.Divisible && expectedUnit.Divisible != unit.Divisible)
                await CheckUnitRelations(false, unit.Id);

            unit.Set(
                expectedUnit.Divisible,
                expectedUnit.ConvFact1,
                expectedUnit.ConvFact2);
        }
    }

    protected void AddUnit(
        UnitGroup unitGroup,
        IList<Unit> units)
    {
        foreach (var unit in units.Where(unit => unit.Id == default))
            AddUnit(unitGroup, unit);
    }
    #endregion

    protected void AddUnit(
        UnitGroup unitGroup,
        Unit unit)
    {
        unitGroup.CheckUnitCodeExists(unit.Code);
        unitGroup.UnitLines.Add(unit);
    }

    public async Task ChangeCodeAsync(
        UnitGroup unitGroup,
        string newCode)
    {
        await CheckUnitGroupExistsAsync(newCode, unitGroup.Id);
        unitGroup.ChangeCode(newCode);
    }
}
