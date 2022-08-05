using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Http.Client;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

namespace MyCompanyName.MyProjectName.Common;

[DependsOn(
    typeof(MyProjectNameCommonApplicationContractsModule),
    typeof(AbpHttpClientModule))]
public class MyProjectNameCommonHttpApiClientModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddHttpClientProxies(
            typeof(MyProjectNameCommonApplicationContractsModule).Assembly,
            MyProjectNameCommonRemoteServiceConsts.RemoteServiceName
        );

        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<MyProjectNameCommonHttpApiClientModule>();
        });

    }
}
