using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Http.Client;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;
using MyCompanyName.MyProjectName.Common;

namespace MyCompanyName.MyProjectName.Public;

[DependsOn(
    typeof(MyProjectNamePublicApplicationContractsModule),
    typeof(MyProjectNameCommonHttpApiClientModule))]
public class MyProjectNamePublicHttpApiClientModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddHttpClientProxies(
            typeof(MyProjectNamePublicApplicationContractsModule).Assembly,
            MyProjectNamePublicRemoteServiceConsts.RemoteServiceName
        );

        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<MyProjectNamePublicHttpApiClientModule>();
        });

    }
}
