using Volo.Abp;

namespace Allegory.Saler.Units;

public  class UnitConvFactMustSetException : BusinessException
{
    public UnitConvFactMustSetException()
        : base(SalerDomainErrorCodes.UnitConvFactMustSet)
    {

    }
}
