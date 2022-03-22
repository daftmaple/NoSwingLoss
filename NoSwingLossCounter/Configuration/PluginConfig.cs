using System.Runtime.CompilerServices;
using IPA.Config.Stores;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace NoSwingLossCounter.Configuration
{
    class PluginConfig
    {
        public static PluginConfig Instance { get; set; }
        public virtual bool separateSaber { get; set; } = false;
        public virtual bool excludeDottedLink { get; set; } = false; 
        public virtual bool normaliseArrowedLink { get; set; } = false;
    }
}