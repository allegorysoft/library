using Volo.Abp.Reflection;

namespace MyCompanyName.MyProjectName.Admin.Permissions;

public class MyProjectNameAdminPermissions
{
    public const string GroupName = "MyProjectName.Admin";

    public static string[] GetAll()
    {
        return ReflectionHelper.GetPublicConstantsRecursively(typeof(MyProjectNameAdminPermissions));
    }
}
