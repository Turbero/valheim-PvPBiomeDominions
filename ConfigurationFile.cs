using BepInEx.Configuration;
using BepInEx;
using System;
using System.IO;
using ServerSync;

namespace PvPBiomeDominions
{
    internal class ConfigurationFile
    {
        public enum Toggle
        {
            On = 1,
            Off = 0
        }
        public enum PvPRule
        {
            Pve = 0,
            Pvp = 1,
            Any = 2
        }
        public enum PositionSharingRule
        {
            Hide = 0,
            Show = 1,
            Any = 2
        }
        
        private static ConfigEntry<bool> _serverConfigLocked = null;

        public static ConfigEntry<bool> debug;

        private static ConfigFile configFile;
        private static readonly string ConfigFileName = PvPBiomeDominions.GUID + ".cfg";
        private static readonly string ConfigFileFullPath = Paths.ConfigPath + Path.DirectorySeparatorChar + ConfigFileName;

        public static readonly ConfigSync ConfigSync = new ConfigSync(PvPBiomeDominions.GUID)
        {
            DisplayName = PvPBiomeDominions.NAME,
            CurrentVersion = PvPBiomeDominions.VERSION,
            MinimumRequiredVersion = PvPBiomeDominions.VERSION
        };
        
        //PVP Management
        public static ConfigEntry<Toggle> pvpOffInWards = null;
        public static ConfigEntry<Toggle> pvpAdminExempt = null;
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
        public static ConfigEntry<Toggle> positionAdminExempt;
        public static ConfigEntry<Toggle> positionOffInWards;
        public static ConfigEntry<PositionSharingRule> positionRuleInMeadows = null;
        public static ConfigEntry<PositionSharingRule> positionRuleInBlackForest = null;
        public static ConfigEntry<PositionSharingRule> positionRuleInSwamp = null;
        public static ConfigEntry<PositionSharingRule> positionRuleInMountain = null;
        public static ConfigEntry<PositionSharingRule> positionRuleInPlains = null;
        public static ConfigEntry<PositionSharingRule> positionRuleInMistlands = null;
        public static ConfigEntry<PositionSharingRule> positionRuleInAshlands = null;
        public static ConfigEntry<PositionSharingRule> positionRuleInDeepNorth = null;
        public static ConfigEntry<PositionSharingRule> positionRuleInOcean = null;

        public static PvPRule getCurrentBiomeRulePvPRule()
        {
            if (!EnvMan.instance) return PvPRule.Any;
            
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

            return PvPRule.Any;
        }
        
        public static PositionSharingRule getCurrentBiomeRulePosition()
        {
            if (!EnvMan.instance) return PositionSharingRule.Any;
            return getBiomeRulePosition(EnvMan.instance.GetCurrentBiome());
        }
        
        
        
        public static PositionSharingRule getBiomeRulePosition(Heightmap.Biome biome)
        {
            if (!EnvMan.instance) return PositionSharingRule.Any;
            
            switch (biome)
            {
                case Heightmap.Biome.Meadows: return positionRuleInMeadows.Value;
                case Heightmap.Biome.BlackForest: return positionRuleInBlackForest.Value;
                case Heightmap.Biome.Swamp: return positionRuleInSwamp.Value;
                case Heightmap.Biome.Mountain: return positionRuleInMountain.Value;
                case Heightmap.Biome.Plains: return positionRuleInPlains.Value;
                case Heightmap.Biome.Mistlands: return positionRuleInMistlands.Value;
                case Heightmap.Biome.AshLands: return positionRuleInAshlands.Value;
                case Heightmap.Biome.DeepNorth: return positionRuleInDeepNorth.Value;
                case Heightmap.Biome.Ocean: return positionRuleInOcean.Value;
            }

            return PositionSharingRule.Any;
        }

        internal static void LoadConfig(BaseUnityPlugin plugin)
        {
            {
                configFile = plugin.Config;

                _serverConfigLocked = config("1 - General", "Lock Configuration", true, "If on, the configuration is locked and can be changed by server admins only.");
                _ = ConfigSync.AddLockingConfigEntry(_serverConfigLocked);

                debug = config("1 - General", "DebugMode", false, "Enabling/Disabling the debugging in the console (default = false)", false);
                
                pvpAdminExempt = config("2 - PvP Settings", "Admin Exempt", Toggle.On, new ConfigDescription("If on, server admins can bypass the pvp biomes rules."));
                pvpOffInWards = config("2 - PvP Settings", "Off In Wards", Toggle.Off, new ConfigDescription("If on, it will not force PvP inside wards."));
                pvpRuleInMeadows = config("2 - PvP Settings", "Biome 1 - Meadows Rule", PvPRule.Pvp, new ConfigDescription("Set up the pvp rule in Meadows. Possible values: Pvp,Pve,Any."));
                pvpRuleInBlackForest = config("2 - PvP Settings", "Biome 2 - Black Forest Rule", PvPRule.Pvp, new ConfigDescription("Set up the pvp rule in Black Forest. Possible values: Pvp,Pve,Any."));
                pvpRuleInSwamp = config("2 - PvP Settings", "Biome 3 - Swamp Rule", PvPRule.Pvp, new ConfigDescription("Set up the pvp rule in Swamp. Possible values: Pvp,Pve,Any."));
                pvpRuleInMountain = config("2 - PvP Settings", "Biome 4 - Mountain Rule", PvPRule.Pvp, new ConfigDescription("Set up the pvp rule in Mountain. Possible values: Pvp,Pve,Any."));
                pvpRuleInPlains = config("2 - PvP Settings", "Biome 5 - Plains Rule", PvPRule.Pvp, new ConfigDescription("Set up the pvp rule in Plains. Possible values: Pvp,Pve,Any."));
                pvpRuleInMistlands = config("2 - PvP Settings", "Biome 6 - Mistlands Rule", PvPRule.Pvp, new ConfigDescription("Set up the pvp rule in Mistlands. Possible values: Pvp,Pve,Any."));
                pvpRuleInAshlands = config("2 - PvP Settings", "Biome 7 - Ashlands Rule", PvPRule.Pvp, new ConfigDescription("Set up the pvp rule in Ashlands. Possible values: Pvp,Pve,Any."));
                pvpRuleInDeepNorth = config("2 - PvP Settings", "Biome 8 - Deep North Rule", PvPRule.Pvp, new ConfigDescription("Set up the pvp rule in Deep North. Possible values: Pvp,Pve,Any."));
                pvpRuleInOcean = config("2 - PvP Settings", "Biome 9 - Ocean Rule", PvPRule.Pvp, new ConfigDescription("Set up the pvp rule in Ocean. Possible values: Pvp,Pve,Any."));

                positionAdminExempt = config("3 - Map Position", "Admin Exempt", Toggle.On, new ConfigDescription("If on, server admins can bypass the 'Position Always On' rule.")); 
                positionOffInWards = config("3 - Map Position", "Off In Wards", Toggle.Off, new ConfigDescription("If on, hide position sharing in wards and force position sharing to be off while inside a ward."));
                positionRuleInMeadows = config("3 - Map Position", "Biome 1 - Meadows Rule", PositionSharingRule.Show, new ConfigDescription("Set up the position sharing rule in Meadows. Possible values: Hide,Show,Any."));
                positionRuleInBlackForest = config("3 - Map Position", "Biome 2 - Black Forest Rule", PositionSharingRule.Show, new ConfigDescription("Set up the position sharing in Black Forest. Possible values: Hide,Show,Any."));
                positionRuleInSwamp = config("3 - Map Position", "Biome 3 - Swamp Rule", PositionSharingRule.Show, new ConfigDescription("Set up the position sharing in Swamp. Possible values: Hide,Show,Any."));
                positionRuleInMountain = config("3 - Map Position", "Biome 4 - Mountain Rule", PositionSharingRule.Show, new ConfigDescription("Set up the position sharing in Mountain. Possible values: Hide,Show,Any."));
                positionRuleInPlains = config("3 - Map Position", "Biome 5 - Plains Rule", PositionSharingRule.Show, new ConfigDescription("Set up the position sharing in Plains. Possible values: Hide,Show,Any."));
                positionRuleInMistlands = config("3 - Map Position", "Biome 6 - Mistlands Rule", PositionSharingRule.Show, new ConfigDescription("Set up the position sharing in Mistlands. Possible values: Hide,Show,Any."));
                positionRuleInAshlands = config("3 - Map Position", "Biome 7 - Ashlands Rule", PositionSharingRule.Show, new ConfigDescription("Set up the position sharing in Ashlands. Possible values: Hide,Show,Any."));
                positionRuleInDeepNorth = config("3 - Map Position", "Biome 8 - Deep North Rule", PositionSharingRule.Show, new ConfigDescription("Set up the position sharing in Deep North. Possible values: Hide,Show,Any."));
                positionRuleInOcean = config("3 - Map Position", "Biome 9 - Ocean Rule", PositionSharingRule.Show, new ConfigDescription("Set up the position sharing in Ocean. Possible values: Hide,Show,Any."));

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
