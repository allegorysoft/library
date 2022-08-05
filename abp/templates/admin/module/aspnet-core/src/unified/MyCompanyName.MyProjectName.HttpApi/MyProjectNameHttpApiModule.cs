using Volo.Abp.Modularity;
using MyCompanyName.MyProjectName.Admin;
using MyCompanyName.MyProjectName.Public;

namespace MyCompanyName.MyProjectName;

[DependsOn(
    typeof(MyProjectNameAdminHttpApiModule),
    typeof(MyProjectNamePublicHttpApiModule),
    typeof(MyProjectNameApplicationContractsModule)
    )]
public class MyProjectNameHttpApiModule : AbpModule
{

}
