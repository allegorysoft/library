using Volo.Abp.Reflection;

namespace MyCompanyName.MyProjectName.Public.Permissions;

public class MyProjectNamePublicPermissions
{
    public const string GroupName = "MyProjectName.Public";

    public static string[] GetAll()
    {
        return ReflectionHelper.GetPublicConstantsRecursively(typeof(MyProjectNamePublicPermissions));
    }
}
