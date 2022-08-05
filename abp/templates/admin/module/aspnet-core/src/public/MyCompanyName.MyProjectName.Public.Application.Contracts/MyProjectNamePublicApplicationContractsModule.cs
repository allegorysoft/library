using MyCompanyName.MyProjectName.Common;
using Volo.Abp.Modularity;

namespace MyCompanyName.MyProjectName.Public;

[DependsOn(
    typeof(MyProjectNameCommonApplicationContractsModule)
    )]
public class MyProjectNamePublicApplicationContractsModule : AbpModule
{

}
