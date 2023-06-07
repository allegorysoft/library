using Allegory.Saler.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace Allegory.Saler.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class SalerController : AbpControllerBase
{
    protected SalerController()
    {
        LocalizationResource = typeof(SalerResource);
    }
}
