using Allegory.Saler.Calculations.Product;
using Allegory.Saler.Units;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json;
using System.Collections.Generic;
using Volo.Abp.AuditLogging;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Emailing;
using Volo.Abp.ExceptionHandling.Localization;
using Volo.Abp.Identity;
using Volo.Abp.IdentityServer;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement.Identity;
using Volo.Abp.PermissionManagement.IdentityServer;
using Volo.Abp.SettingManagement;
using Volo.Abp.VirtualFileSystem;

namespace Allegory.Saler;

[DependsOn(
    typeof(SalerDomainSharedModule),
    typeof(AbpAuditLoggingDomainModule),
    typeof(AbpBackgroundJobsDomainModule),
    typeof(AbpIdentityDomainModule),
    typeof(AbpPermissionManagementDomainIdentityModule),
    typeof(AbpIdentityServerDomainModule),
    typeof(AbpPermissionManagementDomainIdentityServerModule),
    typeof(AbpSettingManagementDomainModule),

    typeof(AbpEmailingModule)
)]
public class SalerDomainModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
#if DEBUG
        context.Services.Replace(ServiceDescriptor.Singleton<IEmailSender, NullEmailSender>());
#endif
        context.Services.AddTransient(typeof(IProductCalculator<,,>), typeof(ProductCalculator<,,>));
        context.Services.AddTransient(typeof(ProductCalculator<,,>), typeof(ProductCalculator<,,>));

        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<SalerDomainModule>();//If we don't add virtual file system to our localization resource then we can't get from virtual files
        });

        Configure<AbpLocalizationOptions>(options =>
        {
            var exceptionHandlingResource = options.Resources.Get<AbpExceptionHandlingResource>();

            exceptionHandlingResource
            .AddVirtualJson("/Volo/Abp/ExceptionHandling/MyLocalization")//This is for our custom exceptions
            .AddVirtualJson("/Allegory/Saler/Localization/Saler");//This is for our domain types (UnitGroup, Client, etc..)
        });

        ConfigureDeduction(context);
        ConfigureGlobalUnitCodes(context);
    }

    void ConfigureDeduction(ServiceConfigurationContext context)
    {
        context.Services.AddSingleton(sp =>
        {
            var virtualFileProvider = context.Services.GetRequiredService<IVirtualFileProvider>();
            var deductionStr = virtualFileProvider.GetFileInfo("/Resources/Deductions.json").ReadAsString();

            return JsonConvert.DeserializeObject<IList<Deduction>>(deductionStr);
        });
    }

    void ConfigureGlobalUnitCodes(ServiceConfigurationContext context)
    {
        context.Services.AddSingleton(sp =>
        {
            var virtualFileProvider = context.Services.GetRequiredService<IVirtualFileProvider>();
            var globalUnitCodeStr = virtualFileProvider.GetFileInfo("/Resources/GlobalUnitCodes.json").ReadAsString();

            return JsonConvert.DeserializeObject<IList<GlobalUnit>>(globalUnitCodeStr);
        });
    }
}
