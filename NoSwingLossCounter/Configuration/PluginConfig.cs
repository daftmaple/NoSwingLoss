using System.Runtime.CompilerServices;
using IPA.Config.Stores;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace NoSwingLossCounter.Configuration
{
    class PluginConfig
    {
        public static PluginConfig Instance { get; set; }
        public virtual bool separateSaber { get; set; } = true;
        public virtual bool excludeDottedLink { get; set; } = false;
        public virtual bool excludeMultiplier { get; set; } = false;
    }
}