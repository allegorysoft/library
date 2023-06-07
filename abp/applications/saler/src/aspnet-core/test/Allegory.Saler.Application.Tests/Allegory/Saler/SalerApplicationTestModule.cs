using Volo.Abp.Modularity;

namespace Allegory.Saler;

[DependsOn(
    typeof(SalerApplicationModule),
    typeof(SalerDomainTestModule)
    )]
public class SalerApplicationTestModule : AbpModule
{

}
