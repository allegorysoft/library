using Volo.Abp;
using Volo.Abp.Domain.Entities;

namespace Allegory.Saler.Units;

public class Unit : Entity<int>
{
    public int UnitGroupId { get; protected set; }
    public string Code { get; protected set; }
    public string Name { get; protected set; }
    public bool MainUnit { get; internal set; }
    public decimal ConvFact1 { get; protected set; }
    public decimal ConvFact2 { get; protected set; }
    public bool Divisible { get; protected set; }
    public string GlobalUnitCode { get; protected set; }

    protected Unit() { }

    public Unit(
        string code,
        decimal convFact1,
        decimal convFact2,
        bool divisible,
        string name = default,
        bool mainUnit = default,
        int id = default,
        string globalUnitCode = default)
    {
        SetCode(code);
        SetName(name);
        MainUnit = mainUnit;//This have to be first set before Set(divisible..) method
        Set(divisible, convFact1, convFact2);
        Id = id;
        SetGlobalUnitCode(globalUnitCode);
    }

    internal void SetCode(string code)
    {
        Check.NotNullOrWhiteSpace(code, nameof(Code), UnitConsts.MaxCodeLength);
        Code = code;
    }

    public void SetName(string name)
    {
        Check.Length(name, nameof(Name), UnitConsts.MaxNameLength);
        Name = name;
    }

    public void SetGlobalUnitCode(string globalUnitCode)
    {
        Check.Length(globalUnitCode, nameof(GlobalUnitCode), UnitConsts.MaxGlobalUnitCodeLength);
        GlobalUnitCode = globalUnitCode;
    }

    internal void Set(
        bool divisible,
        decimal? convFact1 = default,
        decimal? convFact2 = default)
    {
        convFact1 = convFact1 ?? ConvFact1;
        convFact2 = convFact2 ?? ConvFact2;

        if (!divisible
            && ((convFact1 ?? ConvFact1) % 1 != 0
                || (convFact2 ?? ConvFact2) % 1 != 0))
            throw new UnitCannotDividedException(Code);

        if (convFact1 <= 0 || convFact2 <= 0)
            throw new UnitConvFactMustSetException();

        if (MainUnit && (convFact1 != 1 || convFact2 != 1))
            throw new MainUnitConvFactMustOneException();

        Divisible = divisible;
        ConvFact1 = convFact1.Value;
        ConvFact2 = convFact2.Value;
    }

    public void CheckDivisibility(decimal number)
    {
        if (!Divisible && number % 1 != 0)
            throw new UnitCannotDividedException(Code);
    }
}
