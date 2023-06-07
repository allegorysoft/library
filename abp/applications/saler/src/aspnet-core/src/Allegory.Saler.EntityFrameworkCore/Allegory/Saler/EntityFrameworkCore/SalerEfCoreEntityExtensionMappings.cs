using Allegory.Saler.Clients;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Volo.Abp.Identity;
using Volo.Abp.ObjectExtending;
using Volo.Abp.Threading;

namespace Allegory.Saler.EntityFrameworkCore;

public static class SalerEfCoreEntityExtensionMappings
{
    private static readonly OneTimeRunner OneTimeRunner = new OneTimeRunner();

    public static void Configure()
    {
        SalerGlobalFeatureConfigurator.Configure();
        SalerModuleExtensionConfigurator.Configure();

        //OneTimeRunner.Run(() =>
        //{
        //    ObjectExtensionManager.Instance
        //        .MapEfCoreProperty<IdentityUser, int?>(
        //        "ClientId",
        //        (entityBuilder, propertyBuilder) =>
        //        {
        //            var builder = (EntityTypeBuilder<IdentityUser>)entityBuilder;
        //            builder.HasOne<Client>().WithMany().HasForeignKey(
        //                "ClientId"
        //                );
        //        });
        //});
    }
}
