using Volo.Abp.Modularity;
using MyCompanyName.MyProjectName.Admin;
using MyCompanyName.MyProjectName.Public;

namespace MyCompanyName.MyProjectName;

[DependsOn(
    typeof(MyProjectNameAdminHttpApiClientModule),
    typeof(MyProjectNamePublicHttpApiClientModule),
    typeof(MyProjectNameApplicationContractsModule)
    )]
public class MyProjectNameHttpApiClientModule : AbpModule
{
    
}
