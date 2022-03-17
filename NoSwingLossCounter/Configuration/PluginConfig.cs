using System.Runtime.CompilerServices;
using IPA.Config.Stores;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace NoSwingLossCounter.Configuration
{
    internal class PluginConfig
    {
        public static PluginConfig ConfigInstance { get; set; }
        public virtual bool separateSaber { get; set; } = false;
    }
}