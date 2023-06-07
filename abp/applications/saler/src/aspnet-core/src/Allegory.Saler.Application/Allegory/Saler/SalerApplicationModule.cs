using Volo.Abp.Account;
using Volo.Abp.Auditing;
using Volo.Abp.AutoMapper;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.SettingManagement;

namespace Allegory.Saler;

[DependsOn(
    typeof(SalerDomainModule),
    typeof(AbpAccountApplicationModule),
    typeof(SalerApplicationContractsModule),
    typeof(AbpIdentityApplicationModule),
    typeof(AbpPermissionManagementApplicationModule),
    typeof(AbpSettingManagementApplicationModule)
    )]
public class SalerApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        //Configure<AbpExceptionHandlingOptions>(options =>
        //{   
        //    options.SendExceptionsDetailsToClients = true; //Tüm exceptionları user tarafına gösterebiliriz
        //    options.SendStackTraceToClients = false; //Hatanın hangi kod satırında olduğu bilgisini gönderir
        //});

        Configure<AbpAuditingOptions>(options =>
        {
#if DEBUG
            options.IsEnabled = true;
            //options.EntityHistorySelectors.AddAllEntities();
#endif
        });

        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<SalerApplicationModule>();
        });
    }
}
