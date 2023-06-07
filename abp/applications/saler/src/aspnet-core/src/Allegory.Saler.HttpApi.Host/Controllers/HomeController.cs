using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using Volo.Abp.AspNetCore.Mvc;

namespace Allegory.Saler.Controllers;

public class HomeController : AbpController
{
    public ActionResult Index()
    {
        return Redirect("~/swagger");
    }

    [HttpGet("version")]
    public VersionInfo Version()
    {
        return new VersionInfo
        {
            Version = Assembly.GetExecutingAssembly().GetName().Version.ToString(),
            FileVersion = Assembly.GetExecutingAssembly().GetFileVersion(),
        };
    }
}

public class VersionInfo
{
    public string Version { get; set; }
    public string FileVersion { get; set; }
}