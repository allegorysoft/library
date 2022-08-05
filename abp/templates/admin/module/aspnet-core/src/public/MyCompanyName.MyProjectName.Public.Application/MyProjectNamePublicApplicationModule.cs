using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;
using MyCompanyName.MyProjectName.Common;

namespace MyCompanyName.MyProjectName.Public;

[DependsOn(
    typeof(MyProjectNamePublicApplicationContractsModule),
    typeof(MyProjectNameCommonApplicationModule)
    )]
public class MyProjectNamePublicApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAutoMapperObjectMapper<MyProjectNamePublicApplicationModule>();
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<MyProjectNamePublicApplicationModule>(validate: true);
        });
    }
}
