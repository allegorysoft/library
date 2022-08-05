using MyCompanyName.MyProjectName.Common;
using Volo.Abp.Modularity;

namespace MyCompanyName.MyProjectName.Admin;

[DependsOn(
    typeof(MyProjectNameCommonApplicationContractsModule)
    )]
public class MyProjectNameAdminApplicationContractsModule : AbpModule
{

}
