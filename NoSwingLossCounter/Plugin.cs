using IPA;
using IPALogger = IPA.Logging.Logger;
using IPA.Config.Stores;
using Zenject;
using NoSwingLossCounter.Configuration;

namespace NoSwingLossCounter
{
    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
    {
        internal static Plugin Instance { get; private set; }
        internal static string Name => "NoSwingLossCounter";

        [Init]
        public Plugin(IPALogger logger, IPA.Config.Config config)
        {
            PluginConfig.Instance = config.Generated<PluginConfig>();
            Instance = this;
            Logger.log = logger;
            Logger.log.Debug("Logger for NoSwingLoss has been initialised.");
        }

        [OnEnable]
        public void OnEnable() { }

        [OnDisable]
        public void OnDisable() { }
    }
}
