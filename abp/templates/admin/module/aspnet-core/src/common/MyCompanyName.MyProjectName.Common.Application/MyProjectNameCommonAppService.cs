using MyCompanyName.MyProjectName.Localization;
using Volo.Abp.Application.Services;

namespace MyCompanyName.MyProjectName.Common;

public abstract class MyProjectNameCommonAppService : ApplicationService
{
    protected MyProjectNameCommonAppService()
    {
        LocalizationResource = typeof(MyProjectNameResource);
        ObjectMapperContext = typeof(MyProjectNameCommonApplicationModule);
    }
}
