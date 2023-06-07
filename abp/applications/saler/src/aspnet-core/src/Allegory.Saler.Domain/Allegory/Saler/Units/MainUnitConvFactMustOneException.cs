using Volo.Abp;

namespace Allegory.Saler.Units;

public class MainUnitConvFactMustOneException : BusinessException
{
    public MainUnitConvFactMustOneException() 
        : base(SalerDomainErrorCodes.MainUnitConvFactMustOne)
    {

    }
}
