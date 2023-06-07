using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace Allegory.Saler;

[Dependency(ReplaceServices = true)]
public class SalerBrandingProvider : DefaultBrandingProvider
{
    public override string AppName => "Saler";
}
