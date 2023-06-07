namespace Allegory.Saler.Permissions;

public static class SalerPermissions
{
    public static class General
    {
        public const string GroupName = "General";
        public static class Currency
        {
            public const string Default = GroupName + ".Currency";
            public const string Create = Default + ".Create";
            public const string Edit = Default + ".Edit";
            public const string Delete = Default + ".Delete";
        }
    }

    public static class ClientManagement
    {   
        public const string GroupName = "ClientManagement";
        public static class Client
        {
            public const string Default = GroupName + ".Client";
            public const string Create = Default + ".Create";
            public const string Edit = Default + ".Edit";
            public const string Delete = Default + ".Delete";
        }
    }

    public static class ProductManagement
    {
        public const string GroupName = "ProductManagement";
        public static class UnitGroup
        {
            public const string Default = GroupName + ".UnitGroup";
            public const string Create = Default + ".Create";
            public const string Edit = Default + ".Edit";
            public const string Delete = Default + ".Delete";
        }

        public static class Item
        {
            public const string Default = GroupName + ".Item";
            public const string Create = Default + ".Create";
            public const string Edit = Default + ".Edit";
            public const string Delete = Default + ".Delete";
        }

        public static class Service
        {
            public const string Default = GroupName + ".Service";
            public const string Create = Default + ".Create";
            public const string Edit = Default + ".Edit";
            public const string Delete = Default + ".Delete";
        }

        public static class UnitPrice
        {
            public const string Default = GroupName + ".UnitPrice";
            public const string Create = Default + ".Create";
            public const string Edit = Default + ".Edit";
            public const string Delete = Default + ".Delete";
        }
    }

    public static class OrderManagement
    {
        public const string GroupName = "OrderManagement";
        public static class Order
        {
            public const string Default = GroupName + ".Order";
            public const string Create = Default + ".Create";
            public const string Edit = Default + ".Edit";
            public const string Delete = Default + ".Delete";
        }
    }
}
