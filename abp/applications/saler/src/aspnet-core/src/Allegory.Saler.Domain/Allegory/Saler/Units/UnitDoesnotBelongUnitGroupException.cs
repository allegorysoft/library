using Volo.Abp;

namespace Allegory.Saler.Units;

public class UnitDoesnotBelongUnitGroupException : BusinessException
{
    public UnitDoesnotBelongUnitGroupException(
        string unitCode,
        string unitGroupCode) 
        : base(SalerDomainErrorCodes.UnitDoesnotBelongUnitGroup)
    {
        WithData("unitCode", unitCode);
        WithData("unitGroupCode", unitGroupCode);
    }
}
