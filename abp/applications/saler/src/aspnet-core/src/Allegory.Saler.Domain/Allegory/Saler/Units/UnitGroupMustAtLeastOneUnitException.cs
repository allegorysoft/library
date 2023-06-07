using Volo.Abp;

namespace Allegory.Saler.Units;

public class UnitGroupMustAtLeastOneUnitException : BusinessException
{
    public UnitGroupMustAtLeastOneUnitException() 
        : base(SalerDomainErrorCodes.UnitGroupMustAtLeastOneUnit)
    {

    }
}
