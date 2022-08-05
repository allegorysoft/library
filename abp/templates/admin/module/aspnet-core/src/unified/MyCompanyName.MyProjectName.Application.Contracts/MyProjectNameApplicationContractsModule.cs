using Volo.Abp.Modularity;
using MyCompanyName.MyProjectName.Admin;
using MyCompanyName.MyProjectName.Public;

namespace MyCompanyName.MyProjectName;

[DependsOn(
    typeof(MyProjectNameAdminApplicationContractsModule),
    typeof(MyProjectNamePublicApplicationContractsModule)
    )]
public class MyProjectNameApplicationContractsModule : AbpModule
{

}
