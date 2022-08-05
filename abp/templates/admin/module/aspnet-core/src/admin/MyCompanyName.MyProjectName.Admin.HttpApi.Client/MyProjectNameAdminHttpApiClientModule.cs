using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Http.Client;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;
using MyCompanyName.MyProjectName.Common;

namespace MyCompanyName.MyProjectName.Admin;

[DependsOn(
    typeof(MyProjectNameAdminApplicationContractsModule),
    typeof(MyProjectNameCommonHttpApiClientModule))]
public class MyProjectNameAdminHttpApiClientModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddHttpClientProxies(
            typeof(MyProjectNameAdminApplicationContractsModule).Assembly,
            MyProjectNameAdminRemoteServiceConsts.RemoteServiceName
        );

        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<MyProjectNameAdminHttpApiClientModule>();
        });

    }
}
