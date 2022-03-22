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
            set => PluginConfig.Instance.separateSaber = value;
        }

        [UIValue("excludeDottedLink")]
        public bool excludeDottedLink
        {
            get => PluginConfig.Instance.excludeDottedLink;
            set => PluginConfig.Instance.excludeDottedLink = value;
        }

        [UIValue("normalArrowedLink")]
        public bool normalArrowedLink
        {
            get => PluginConfig.Instance.normalArrowedLink;
            set => PluginConfig.Instance.normalArrowedLink = value;
        }
    }
}
