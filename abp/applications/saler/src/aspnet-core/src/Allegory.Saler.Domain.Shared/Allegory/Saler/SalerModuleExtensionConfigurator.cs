using System.ComponentModel.DataAnnotations;
using Volo.Abp.Identity;
using Volo.Abp.ObjectExtending;
using Volo.Abp.Threading;

namespace Allegory.Saler;

public static class SalerModuleExtensionConfigurator
{
    private static readonly OneTimeRunner OneTimeRunner = new OneTimeRunner();

    public static void Configure()
    {
        OneTimeRunner.Run(() =>
        {
            ConfigureExistingProperties();
            ConfigureExtraProperties();
        });
    }

    private static void ConfigureExistingProperties()
    {
        /* You can change max lengths for properties of the
         * entities defined in the modules used by your application.
         *
         * Example: Change user and role name max lengths

           IdentityUserConsts.MaxNameLength = 99;
           IdentityRoleConsts.MaxNameLength = 99;

         * Notice: It is not suggested to change property lengths
         * unless you really need it. Go with the standard values wherever possible.
         *
         * If you are using EF Core, you will need to run the add-migration command after your changes.
         */
    }

    private static void ConfigureExtraProperties()
    {
        //ObjectExtensionManager.Instance.Modules()
        //   .ConfigureIdentity(identity =>
        //   {
        //       identity.ConfigureUser(user =>
        //       {
        //           user.AddOrUpdateProperty<int?>( 
        //               "ClientId",
        //               property =>
        //               {
        //                   property.UI.OnTable.IsVisible =
        //                   property.UI.OnCreateForm.IsVisible = 
        //                   property.UI.OnEditForm.IsVisible = false;
        //               }
        //           );
        //       });
        //   });
    }
}
