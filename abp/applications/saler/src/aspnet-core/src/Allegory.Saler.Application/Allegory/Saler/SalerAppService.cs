using Allegory.Saler.Localization;
using Volo.Abp.Application.Services;

namespace Allegory.Saler;

public abstract class SalerAppService : ApplicationService
{
    protected SalerAppService()
    {
        LocalizationResource = typeof(SalerResource);
    }
}
