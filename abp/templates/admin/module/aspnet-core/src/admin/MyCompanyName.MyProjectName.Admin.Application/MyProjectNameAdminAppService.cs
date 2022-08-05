using MyCompanyName.MyProjectName.Localization;
using Volo.Abp.Application.Services;

namespace MyCompanyName.MyProjectName.Admin;

public abstract class MyProjectNameAdminAppService : ApplicationService
{
    protected MyProjectNameAdminAppService()
    {
        LocalizationResource = typeof(MyProjectNameResource);
        ObjectMapperContext = typeof(MyProjectNameAdminApplicationModule);
    }
}
