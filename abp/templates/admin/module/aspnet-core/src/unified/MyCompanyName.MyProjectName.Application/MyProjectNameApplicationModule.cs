using Volo.Abp.Modularity;
using MyCompanyName.MyProjectName.Admin;
using MyCompanyName.MyProjectName.Public;

namespace MyCompanyName.MyProjectName;

[DependsOn(
    typeof(MyProjectNameAdminApplicationModule),
    typeof(MyProjectNamePublicApplicationModule),
    typeof(MyProjectNameApplicationContractsModule)
    )]
public class MyProjectNameApplicationModule : AbpModule
{

}
