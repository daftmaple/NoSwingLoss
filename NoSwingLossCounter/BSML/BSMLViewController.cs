using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using NoSwingLossCounter.Configuration;

namespace NoSwingLossCounter.BSML
{
    [HotReload(RelativePathToLayout = @"BSMLViewController.bsml")]
    [ViewDefinition("NoSwingLossCounter.BSML.BSMLViewController.bsml")]
    internal class BSMLViewController : BSMLAutomaticViewController
    {
        [UIValue("separateSaber")]
        public bool separateSaber
        {
            get => PluginConfig.ConfigInstance.separateSaber;
            set
            {
                PluginConfig.ConfigInstance.separateSaber = value;
            }
        }
    }
}
