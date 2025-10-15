using BepInEx.Configuration;
using BepInEx;
using System;
using System.IO;
using ServerSync;

namespace PvPBiomeDominions
{
    internal class ConfigurationFile
    {
        private static ConfigEntry<bool> _serverConfigLocked = null;

        public static ConfigEntry<bool> debug;
        

        private static ConfigFile configFile;
        private static readonly string ConfigFileName = PvPBiomeDominions.GUID + ".cfg";
        private static readonly string ConfigFileFullPath = Paths.ConfigPath + Path.DirectorySeparatorChar + ConfigFileName;

        private static readonly ConfigSync ConfigSync = new ConfigSync(PvPBiomeDominions.GUID)
        {
            DisplayName = PvPBiomeDominions.NAME,
            CurrentVersion = PvPBiomeDominions.VERSION,
            MinimumRequiredVersion = PvPBiomeDominions.VERSION
        };
        
        //PVP Management
        public static ConfigEntry<Toggle> offPvPInWards = null;
        public static ConfigEntry<PvPRule> pvpRuleInMeadows = null;
        public static ConfigEntry<PvPRule> pvpRuleInBlackForest = null;
        public static ConfigEntry<PvPRule> pvpRuleInSwamp = null;
        public static ConfigEntry<PvPRule> pvpRuleInMountain = null;
        public static ConfigEntry<PvPRule> pvpRuleInPlains = null;
        public static ConfigEntry<PvPRule> pvpRuleInMistlands = null;
        public static ConfigEntry<PvPRule> pvpRuleInAshlands = null;
        public static ConfigEntry<PvPRule> pvpRuleInDeepNorth = null;
        public static ConfigEntry<PvPRule> pvpRuleInOcean = null;
        
        //Position management
        
        
        public enum Toggle
        {
            On = 1,
            Off = 0
        }
    
        public enum PvPRule
        {
            Pve = 0,
            Pvp = 1,
            Free = 1
        }

        public static PvPRule getCurrentBiomeRule()
        {
            Heightmap.Biome biome = EnvMan.instance.GetCurrentBiome();
            switch (biome)
            {
                case Heightmap.Biome.Meadows: return pvpRuleInMeadows.Value;
                case Heightmap.Biome.BlackForest: return pvpRuleInBlackForest.Value;
                case Heightmap.Biome.Swamp: return pvpRuleInSwamp.Value;
                case Heightmap.Biome.Mountain: return pvpRuleInMountain.Value;
                case Heightmap.Biome.Plains: return pvpRuleInPlains.Value;
                case Heightmap.Biome.Mistlands: return pvpRuleInMistlands.Value;
                case Heightmap.Biome.AshLands: return pvpRuleInAshlands.Value;
                case Heightmap.Biome.DeepNorth: return pvpRuleInDeepNorth.Value;
                case Heightmap.Biome.Ocean: return pvpRuleInOcean.Value;
            }

            return PvPRule.Free;
        }

        internal static void LoadConfig(BaseUnityPlugin plugin)
        {
            {
                configFile = plugin.Config;

                _serverConfigLocked = config("1 - General", "Lock Configuration", true, "If on, the configuration is locked and can be changed by server admins only.");
                _ = ConfigSync.AddLockingConfigEntry(_serverConfigLocked);

                debug = config("1 - General", "DebugMode", false, "Enabling/Disabling the debugging in the console (default = false)", false);
                
                /* General */
                offPvPInWards = config("2 - PvP Settings", "Off In Wards", Toggle.Off, "Toggle this on to disable the enforcement of PvP in wards.");
        
                pvpRuleInMeadows = config("2 - PvP Settings", "Biome 1 - Meadows Rule", PvPRule.Pvp, new ConfigDescription("Set up the pvp rule in Meadows. Possible values: Pvp,Pve,Free."));
                pvpRuleInBlackForest = config("2 - PvP Settings", "Biome 2 - Black Forest Rule", PvPRule.Pvp, new ConfigDescription("Set up the pvp rule in Black Forest. Possible values: Pvp,Pve,Free."));
                pvpRuleInSwamp = config("2 - PvP Settings", "Biome 3 - Swamp Rule", PvPRule.Pvp, new ConfigDescription("Set up the pvp rule in Swamp. Possible values: Pvp,Pve,Free."));
                pvpRuleInMountain = config("2 - PvP Settings", "Biome 4 - Mountain Rule", PvPRule.Pvp, new ConfigDescription("Set up the pvp rule in Mountain. Possible values: Pvp,Pve,Free."));
                pvpRuleInPlains = config("2 - PvP Settings", "Biome 5 - Plains Rule", PvPRule.Pvp, new ConfigDescription("Set up the pvp rule in Plains. Possible values: Pvp,Pve,Free."));
                pvpRuleInMistlands = config("2 - PvP Settings", "Biome 6 - Mistlands Rule", PvPRule.Pvp, new ConfigDescription("Set up the pvp rule in Mistlands. Possible values: Pvp,Pve,Free."));
                pvpRuleInAshlands = config("2 - PvP Settings", "Biome 7 - Ashlands Rule", PvPRule.Pvp, new ConfigDescription("Set up the pvp rule in Ashlands. Possible values: Pvp,Pve,Free."));
                pvpRuleInDeepNorth = config("2 - PvP Settings", "Biome 8 - Deep North Rule", PvPRule.Pvp, new ConfigDescription("Set up the pvp rule in Deep North. Possible values: Pvp,Pve,Free."));
                pvpRuleInOcean = config("2 - PvP Settings", "Biome 9 - Ocean Rule", PvPRule.Pvp, new ConfigDescription("Set up the pvp rule in Ocean. Possible values: Pvp,Pve,Free."));

                SetupWatcher();
            }
        }

        private static void SetupWatcher()
        {
            FileSystemWatcher watcher = new FileSystemWatcher(Paths.ConfigPath, ConfigFileName);
            watcher.Changed += ReadConfigValues;
            watcher.Created += ReadConfigValues;
            watcher.Renamed += ReadConfigValues;
            watcher.IncludeSubdirectories = true;
            watcher.SynchronizingObject = ThreadingHelper.SynchronizingObject;
            watcher.EnableRaisingEvents = true;
        }

        private static void ReadConfigValues(object sender, FileSystemEventArgs e)
        {
            if (!File.Exists(ConfigFileFullPath)) return;
            try
            {
                Logger.Log("Attempting to reload configuration...");
                configFile.Reload();
                SettingsChanged(null, null);
            }
            catch
            {
                Logger.LogError($"There was an issue loading {ConfigFileName}");
            }
        }

        private static void SettingsChanged(object sender, EventArgs e)
        {
            
        }

        private static ConfigEntry<T> config<T>(string group, string name, T value, string description,
            bool synchronizedSetting = true)
        {
            return config(group, name, value, new ConfigDescription(description), synchronizedSetting);
        }

        private static ConfigEntry<T> config<T>(string group, string name, T value, ConfigDescription description,
            bool synchronizedSetting = true)
        {
            ConfigDescription extendedDescription =
                new ConfigDescription(
                    description.Description +
                    (synchronizedSetting ? " [Synced with Server]" : " [Not Synced with Server]"),
                    description.AcceptableValues, description.Tags);
            ConfigEntry<T> configEntry = configFile.Bind(group, name, value, extendedDescription);
            //var configEntry = Config.Bind(group, name, value, description);

            SyncedConfigEntry<T> syncedConfigEntry = ConfigSync.AddConfigEntry(configEntry);
            syncedConfigEntry.SynchronizedConfig = synchronizedSetting;

            return configEntry;
        }
    }
}
