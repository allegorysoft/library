using Volo.Abp;

namespace Allegory.Saler.Units;

public class UnitCannotDividedException : BusinessException
{
    public UnitCannotDividedException(string unitCode)
        : base(SalerDomainErrorCodes.UnitCannotDivided)
    {
        WithData("unitCode", unitCode);
    }
}
