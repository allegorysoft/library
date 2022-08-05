using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;
using Volo.Abp.Application;
using MyCompanyName.MyProjectName.Common;

namespace MyCompanyName.MyProjectName.Admin;

[DependsOn(
    typeof(MyProjectNameAdminApplicationContractsModule),
    typeof(MyProjectNameCommonApplicationModule)
    )]
public class MyProjectNameAdminApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAutoMapperObjectMapper<MyProjectNameAdminApplicationModule>();
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<MyProjectNameAdminApplicationModule>(validate: true);
        });
    }
}
