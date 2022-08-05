using Volo.Abp.Application;
using Volo.Abp.Modularity;
using Volo.Abp.Authorization;

namespace MyCompanyName.MyProjectName.Common;

[DependsOn(
    typeof(MyProjectNameDomainSharedModule),
    typeof(AbpDddApplicationContractsModule),
    typeof(AbpAuthorizationModule)
    )]
public class MyProjectNameCommonApplicationContractsModule : AbpModule
{

}
