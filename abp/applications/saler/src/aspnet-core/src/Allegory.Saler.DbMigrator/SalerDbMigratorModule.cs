using Allegory.Saler.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Modularity;

namespace Allegory.Saler.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(SalerEntityFrameworkCoreModule),
    typeof(SalerApplicationContractsModule)
    )]
public class SalerDbMigratorModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpBackgroundJobOptions>(options => options.IsJobExecutionEnabled = false);
    }
}
