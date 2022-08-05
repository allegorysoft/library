using MyCompanyName.MyProjectName.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace MyCompanyName.MyProjectName.Admin;

public abstract class MyProjectNameAdminController : AbpControllerBase
{
    protected MyProjectNameAdminController()
    {
        LocalizationResource = typeof(MyProjectNameResource);
    }
}
