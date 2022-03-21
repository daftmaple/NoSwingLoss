using BeatSaberMarkupLanguage.Attributes;
using NoSwingLossCounter.Configuration;

namespace NoSwingLossCounter.BSML
{
    class Settings
    {
        [UIValue("separateSaber")]
        public bool separateSaber
        {
            get => PluginConfig.Instance.separateSaber;
            set
            {
                PluginConfig.Instance.separateSaber = value;
            }
        }
    }
}
