using MyCompanyName.MyProjectName.Localization;
using Volo.Abp.Application.Services;

namespace MyCompanyName.MyProjectName.Public;

public abstract class MyProjectNamePublicAppService : ApplicationService
{
    protected MyProjectNamePublicAppService()
    {
        LocalizationResource = typeof(MyProjectNameResource);
        ObjectMapperContext = typeof(MyProjectNamePublicApplicationModule);
    }
}
