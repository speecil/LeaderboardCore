using IPA;
using LeaderboardCore.Installers;
using SiraUtil.Zenject;
using System;
using System.Reflection;
using IPA.Config.Stores;
using IPA.Loader;
using LeaderboardCore.Configuration;
using IPALogger = IPA.Logging.Logger;
using IPAConfig = IPA.Config.Config;

namespace LeaderboardCore
{
    /// <summary>
    /// The LeaderboardCore plugin
    /// </summary>
    [Plugin(RuntimeOptions.DynamicInit), NoEnableDisable]
    public class Plugin
    {
        private const string kHarmonyID = "com.github.rithik-b.LeaderboardCore";
        private static readonly HarmonyLib.Harmony harmony = new HarmonyLib.Harmony(kHarmonyID);

        internal static Plugin Instance { get; set; }
        internal static IPALogger Log { get; private set; }

        /// <summary>
        /// Called when the plugin is first loaded by IPA (either when the game starts or when the plugin is enabled if it starts disabled).
        /// [Init] methods that use a Constructor or called before regular methods like InitWithConfig.
        /// Only use [Init] with one Constructor.
        /// </summary>
        [Init]
        public Plugin(IPALogger logger, Zenjector zenjector, IPAConfig config)
        {
            Instance = this;
            Log = logger;
            zenjector.Install<LeaderboardCoreMenuInstaller>(Location.Menu, config.Generated<PluginConfig>());
        }
    }
}
