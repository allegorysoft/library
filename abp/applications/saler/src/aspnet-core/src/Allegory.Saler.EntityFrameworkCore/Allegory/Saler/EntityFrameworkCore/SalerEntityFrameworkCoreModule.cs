using Allegory.Saler.Clients;
using Allegory.Saler.ClientUsers;
using Allegory.Saler.Currencies;
using Allegory.Saler.Items;
using Allegory.Saler.Orders;
using Allegory.Saler.Services;
using Allegory.Saler.UnitPrices;
using Allegory.Saler.Units;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.SqlServer;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.IdentityServer.EntityFrameworkCore;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;

namespace Allegory.Saler.EntityFrameworkCore;

[DependsOn(
    typeof(SalerDomainModule),
    typeof(AbpIdentityEntityFrameworkCoreModule),
    typeof(AbpIdentityServerEntityFrameworkCoreModule),
    typeof(AbpPermissionManagementEntityFrameworkCoreModule),
    typeof(AbpSettingManagementEntityFrameworkCoreModule),
    typeof(AbpEntityFrameworkCoreSqlServerModule),
    typeof(AbpBackgroundJobsEntityFrameworkCoreModule),
    typeof(AbpAuditLoggingEntityFrameworkCoreModule)
    )]
public class SalerEntityFrameworkCoreModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        SalerEfCoreEntityExtensionMappings.Configure();
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAbpDbContext<SalerDbContext>(options =>
        {
            options.AddDefaultRepositories(includeAllEntities: true);

            options.AddRepository<UnitGroup, EfCoreUnitGroupRepository>();
            options.AddRepository<Client, EfCoreClientRepository>();
            options.AddRepository<ClientUser, EfCoreClientUserRepository>();
            options.AddRepository<Item, EfCoreItemRepository>();
            options.AddRepository<ItemStockTransaction, EfCoreItemStockTransactionRepository>();
            options.AddRepository<Service, EfCoreServiceRepository>();
            options.AddRepository<Order, EfCoreOrderRepository>();
            options.AddRepository<CurrencyDailyExchange, EfCoreCurrencyDailyExchangeRepository>();
            options.AddRepository<Currency, EfCoreCurrencyRepository>();
            options.AddRepository<UnitPrice, EfCoreUnitPriceRepository>();
        });

        Configure<AbpDbContextOptions>(options =>
        {
            options.UseSqlServer(
                //p => p.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery)
                );
        });
    }
}
