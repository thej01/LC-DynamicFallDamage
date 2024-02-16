using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx;
using BepInEx.Logging;
using DynamicFallDamage.Patches;
using HarmonyLib;
using static DynamicFallDamage.Logger;


namespace DynamicFallDamage
{

    public class Logger
    {
        internal ManualLogSource MLS;

        public string modName = "No-Name";
        public string modVersion = "No-Ver";

        public enum LogLevelConfig
        {
            None,
            Important,
            Everything
        }

        public void Init(string modGUID = "")
        {
            MLS = BepInEx.Logging.Logger.CreateLogSource(modGUID);
        }

        public bool LogLevelAllow(LogLevelConfig severity = LogLevelConfig.Important, LogLevelConfig severity2 = LogLevelConfig.Everything)
        {
            if (severity2 == LogLevelConfig.None)
                return false;

            if (severity == LogLevelConfig.Everything)
            {
                return severity2 == LogLevelConfig.Everything;
            }

            return true;
        }

        public void Log(string text = "", BepInEx.Logging.LogLevel level = BepInEx.Logging.LogLevel.Info, LogLevelConfig severity = LogLevelConfig.Important)
        {
            bool allowed = true; // ConfigValues.logLevel == null;
            /*if (!allowed)
            {
                allowed = LogLevelAllow(severity, ConfigValues.logLevel);
            }*/

            if (allowed)
            {
                string resultText = string.Format("[{0} v{1}] - {2}", modName, modVersion, text);
                MLS.Log(level, resultText);
            }
        }
    }


    [BepInPlugin(modGUID, modName, modVersion)]
    public class DynamicFallDamageMod : BaseUnityPlugin
    {
        private const string modGUID = "thej01.lc.DynamicFallDamage";
        private const string modName = "DynamicFallDamage";
        private const string modVersion = "1.0.0";

        private readonly Harmony harmony = new Harmony(modGUID);

        private static DynamicFallDamageMod Instance;

        public static Logger fallLogger = new Logger();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            fallLogger.Init(modGUID);

            fallLogger.modName = modName;
            fallLogger.modVersion = modVersion;

            fallLogger.Log("fallLogger Initialised!", BepInEx.Logging.LogLevel.Info, LogLevelConfig.Everything);

            fallLogger.Log("Patching DynamicFallDamageMod...", BepInEx.Logging.LogLevel.Info, LogLevelConfig.Everything);
            harmony.PatchAll(typeof(DynamicFallDamageMod));
            fallLogger.Log("Patched OmegaCore.", BepInEx.Logging.LogLevel.Info, LogLevelConfig.Everything);

            fallLogger.Log("Patching PlayerControllerBPatch...", BepInEx.Logging.LogLevel.Info, LogLevelConfig.Everything);
            harmony.PatchAll(typeof(PlayerControllerBPatch));
            fallLogger.Log("Patched PlayerControllerBPatch.", BepInEx.Logging.LogLevel.Info, LogLevelConfig.Everything);
        }

        public static int fallDamageMin = 30;
        public static int fallDamageMax = 200;

        public static float fallValueRangeMin = 35;
        public static float fallValueRangeMax = 100;
    }
}
