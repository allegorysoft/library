using Localization.Resources.AbpUi;
using MyCompanyName.MyProjectName.Localization;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Microsoft.Extensions.DependencyInjection;
using MyCompanyName.MyProjectName.Common;

namespace MyCompanyName.MyProjectName.Admin;

[DependsOn(
    typeof(MyProjectNameAdminApplicationContractsModule),
    typeof(MyProjectNameCommonHttpApiModule))]
public class MyProjectNameAdminHttpApiModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        PreConfigure<IMvcBuilder>(mvcBuilder =>
        {
            mvcBuilder.AddApplicationPartIfNotExists(typeof(MyProjectNameAdminHttpApiModule).Assembly);
        });
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Get<MyProjectNameResource>()
                .AddBaseTypes(typeof(AbpUiResource));
        });
    }
}
