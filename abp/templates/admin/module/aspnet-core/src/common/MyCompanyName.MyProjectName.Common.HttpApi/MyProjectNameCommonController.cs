using MyCompanyName.MyProjectName.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace MyCompanyName.MyProjectName.Common;

public abstract class MyProjectNameCommonController : AbpControllerBase
{
    protected MyProjectNameCommonController()
    {
        LocalizationResource = typeof(MyProjectNameResource);
    }
}
