using Volo.Abp;

namespace Allegory.Saler.Units;

public class UnitGroupMustOneMainUnitException : BusinessException
{
    public UnitGroupMustOneMainUnitException()
        : base(SalerDomainErrorCodes.UnitGroupMustOneMainUnit)
    {

    }
}
