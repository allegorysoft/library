using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using Volo.Abp;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;

namespace Allegory.Saler.Units;

public class UnitGroup : AuditedAggregateRoot<int>, ICode
{
    public string Code { get; protected set; }

    public string Name { get; protected set; }

    internal IList<Unit> UnitLines;
    public IReadOnlyList<Unit> Units
    {
        get => (IReadOnlyList<Unit>)UnitLines;
        protected set => UnitLines = (IList<Unit>)value;
    }

    protected UnitGroup() { }

    internal UnitGroup(
        string code,
        string name = default)
    {
        SetCode(code);
        SetName(name);
        Units = new Collection<Unit>();
    }

    internal UnitGroup ChangeCode(string code)
    {
        SetCode(code);
        return this;
    }

    private void SetCode(string code)
    {
        Check.NotNullOrWhiteSpace(code, nameof(Code), UnitGroupConsts.MaxCodeLength);
        Code = code;
    }

    public void SetName(string name)
    {
        Check.Length(name, nameof(Name), UnitGroupConsts.MaxNameLength);
        Name = name;
    }

    public void CheckUnitCodeExists(
        string unitCode,
        int? unitId = default)
    {
        Expression<Func<Unit, bool>> expression = unit => unit.Code == unitCode;

        if (unitId != default)
            expression = expression.And(unitGroup => unitGroup.Id != unitId);

        if (Units.Any(expression.Compile()))
            throw new CodeAlreadyExistsException(typeof(Unit), unitCode);
    }
}
