using Allegory.Saler.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace Allegory.Saler.Permissions;

public class SalerPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var clientGroup = context.AddGroup(SalerPermissions.ClientManagement.GroupName, L("Permission:ClientManagement"));
        var clientGroupPermission = clientGroup.AddPermission(SalerPermissions.ClientManagement.Client.Default, L("Permission:ClientManagement:Client"));
        clientGroupPermission.AddChild(SalerPermissions.ClientManagement.Client.Create, L("Permission:ClientManagement:Client.Create"));
        clientGroupPermission.AddChild(SalerPermissions.ClientManagement.Client.Edit, L("Permission:ClientManagement:Client.Edit"));
        clientGroupPermission.AddChild(SalerPermissions.ClientManagement.Client.Delete, L("Permission:ClientManagement:Client.Delete"));

        var productManagementGroup = context.AddGroup(SalerPermissions.ProductManagement.GroupName, L("Permission:ProductManagement"));

        var unitGroupPermission = productManagementGroup.AddPermission(SalerPermissions.ProductManagement.UnitGroup.Default, L("Permission:ProductManagement:UnitGroup"));
        unitGroupPermission.AddChild(SalerPermissions.ProductManagement.UnitGroup.Create, L("Permission:ProductManagement:UnitGroup.Create"));
        unitGroupPermission.AddChild(SalerPermissions.ProductManagement.UnitGroup.Edit, L("Permission:ProductManagement:UnitGroup.Edit"));
        unitGroupPermission.AddChild(SalerPermissions.ProductManagement.UnitGroup.Delete, L("Permission:ProductManagement:UnitGroup.Delete"));

        var itemPermission = productManagementGroup.AddPermission(SalerPermissions.ProductManagement.Item.Default, L("Permission:ProductManagement:Item"));
        itemPermission.AddChild(SalerPermissions.ProductManagement.Item.Create, L("Permission:ProductManagement:Item.Create"));
        itemPermission.AddChild(SalerPermissions.ProductManagement.Item.Edit, L("Permission:ProductManagement:Item.Edit"));
        itemPermission.AddChild(SalerPermissions.ProductManagement.Item.Delete, L("Permission:ProductManagement:Item.Delete"));

        var servicePermission = productManagementGroup.AddPermission(SalerPermissions.ProductManagement.Service.Default, L("Permission:ProductManagement:Service"));
        servicePermission.AddChild(SalerPermissions.ProductManagement.Service.Create, L("Permission:ProductManagement:Service.Create"));
        servicePermission.AddChild(SalerPermissions.ProductManagement.Service.Edit, L("Permission:ProductManagement:Service.Edit"));
        servicePermission.AddChild(SalerPermissions.ProductManagement.Service.Delete, L("Permission:ProductManagement:Service.Delete"));

        var unitPricePermission = productManagementGroup.AddPermission(SalerPermissions.ProductManagement.UnitPrice.Default, L("Permission:ProductManagement:UnitPrice"));
        unitPricePermission.AddChild(SalerPermissions.ProductManagement.UnitPrice.Create, L("Permission:ProductManagement:UnitPrice.Create"));
        unitPricePermission.AddChild(SalerPermissions.ProductManagement.UnitPrice.Edit, L("Permission:ProductManagement:UnitPrice.Edit"));
        unitPricePermission.AddChild(SalerPermissions.ProductManagement.UnitPrice.Delete, L("Permission:ProductManagement:UnitPrice.Delete"));        

        var orderGroup = context.AddGroup(SalerPermissions.OrderManagement.GroupName, L("Permission:OrderManagement"));
        var orderGroupPermission = orderGroup.AddPermission(SalerPermissions.OrderManagement.Order.Default, L("Permission:OrderManagement:Order"));
        orderGroupPermission.AddChild(SalerPermissions.OrderManagement.Order.Create, L("Permission:OrderManagement:Order.Create"));
        orderGroupPermission.AddChild(SalerPermissions.OrderManagement.Order.Edit, L("Permission:OrderManagement:Order.Edit"));
        orderGroupPermission.AddChild(SalerPermissions.OrderManagement.Order.Delete, L("Permission:OrderManagement:Order.Delete"));

        var generalGroup = context.AddGroup(SalerPermissions.General.GroupName, L("Permission:General"));
        var currencyGroupPermission = generalGroup.AddPermission(SalerPermissions.General.Currency.Default, L("Permission:General:Currency"));
        currencyGroupPermission.AddChild(SalerPermissions.General.Currency.Create, L("Permission:General:Currency.Create"));
        currencyGroupPermission.AddChild(SalerPermissions.General.Currency.Edit, L("Permission:General:Currency.Edit"));
        currencyGroupPermission.AddChild(SalerPermissions.General.Currency.Delete, L("Permission:General:Currency.Delete"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<SalerResource>(name);
    }
}
