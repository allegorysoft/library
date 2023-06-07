using Volo.Abp.Settings;

namespace Allegory.Saler.Settings;

public class SalerSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        //context.Add(new SettingDefinition(SalerSettings.MySetting1));
    }
}
