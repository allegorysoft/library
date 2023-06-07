using Allegory.Saler.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace Allegory.Saler;

[DependsOn(
    typeof(SalerEntityFrameworkCoreTestModule)
    )]
public class SalerDomainTestModule : AbpModule
{

}
