using Volo.Abp.Reflection;

namespace MyCompanyName.MyProjectName.Common.Permissions;

public class MyProjectNameCommonPermissions
{
    public const string GroupName = "MyProjectName.Common";

    public static string[] GetAll()
    {
        return ReflectionHelper.GetPublicConstantsRecursively(typeof(MyProjectNameCommonPermissions));
    }
}
