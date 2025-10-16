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
        public enum PvPBiomeRule
        {
            Pve = 0,
            Pvp = 1,
            Any = 2
        }
        public enum PvPWardRule
        {
            Pve = 0,
            Pvp = 1,
            Any = 2,
            Biome = 3
        }
        public enum PositionSharingBiomeRule
        {
            Hide = 0,
            Show = 1,
            Any = 2
        }
        public enum PositionSharingWardRule
        {
            Hide = 0,
            Show = 1,
            Any = 2,
            Biome = 3
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
        public static ConfigEntry<Toggle> pvpAdminExempt = null;
        public static ConfigEntry<PvPWardRule> pvpRuleInWards = null;
        public static ConfigEntry<PvPBiomeRule> pvpRuleInMeadows = null;
        public static ConfigEntry<PvPBiomeRule> pvpRuleInBlackForest = null;
        public static ConfigEntry<PvPBiomeRule> pvpRuleInSwamp = null;
        public static ConfigEntry<PvPBiomeRule> pvpRuleInMountain = null;
        public static ConfigEntry<PvPBiomeRule> pvpRuleInPlains = null;
        public static ConfigEntry<PvPBiomeRule> pvpRuleInMistlands = null;
        public static ConfigEntry<PvPBiomeRule> pvpRuleInAshlands = null;
        public static ConfigEntry<PvPBiomeRule> pvpRuleInDeepNorth = null;
        public static ConfigEntry<PvPBiomeRule> pvpRuleInOcean = null;

        //Position management
        public static ConfigEntry<Toggle> positionAdminExempt = null;
        public static ConfigEntry<PositionSharingWardRule> positionRuleInWards = null;
        public static ConfigEntry<PositionSharingBiomeRule> positionRuleInMeadows = null;
        public static ConfigEntry<PositionSharingBiomeRule> positionRuleInBlackForest = null;
        public static ConfigEntry<PositionSharingBiomeRule> positionRuleInSwamp = null;
        public static ConfigEntry<PositionSharingBiomeRule> positionRuleInMountain = null;
        public static ConfigEntry<PositionSharingBiomeRule> positionRuleInPlains = null;
        public static ConfigEntry<PositionSharingBiomeRule> positionRuleInMistlands = null;
        public static ConfigEntry<PositionSharingBiomeRule> positionRuleInAshlands = null;
        public static ConfigEntry<PositionSharingBiomeRule> positionRuleInDeepNorth = null;
        public static ConfigEntry<PositionSharingBiomeRule> positionRuleInOcean = null;

        public static PvPBiomeRule getCurrentBiomeRulePvPRule()
        {
            if (!EnvMan.instance) return PvPBiomeRule.Any;
            
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

            return PvPBiomeRule.Any;
        }
        
        public static PositionSharingBiomeRule getCurrentBiomeRulePosition()
        {
            if (!EnvMan.instance) return PositionSharingBiomeRule.Any;
            return getBiomeRulePosition(EnvMan.instance.GetCurrentBiome());
        }
        
        
        
        public static PositionSharingBiomeRule getBiomeRulePosition(Heightmap.Biome biome)
        {
            if (!EnvMan.instance) return PositionSharingBiomeRule.Any;
            
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

            return PositionSharingBiomeRule.Any;
        }

        internal static void LoadConfig(BaseUnityPlugin plugin)
        {
            {
                configFile = plugin.Config;

                _serverConfigLocked = config("1 - General", "Lock Configuration", true, "If on, the configuration is locked and can be changed by server admins only.");
                _ = ConfigSync.AddLockingConfigEntry(_serverConfigLocked);

                debug = config("1 - General", "DebugMode", false, "Enabling/Disabling the debugging in the console (default = false)", false);
                
                pvpAdminExempt = config("2 - PvP Settings", "Admin Exempt", Toggle.On, new ConfigDescription("If on, server admins can bypass the pvp biomes rules."));
                pvpRuleInWards = config("2 - PvP Settings", "PvP Rule In Wards", PvPWardRule.Biome, new ConfigDescription("Set up the pvp rule inside wards, overriding biome rules if it's needed. Possible values: Pvp,Pve,Any,Biome"));
                pvpRuleInMeadows = config("2 - PvP Settings", "Biome 1 - Meadows Rule", PvPBiomeRule.Pvp, new ConfigDescription("Set up the pvp rule in Meadows. Possible values: Pvp,Pve,Any."));
                pvpRuleInBlackForest = config("2 - PvP Settings", "Biome 2 - Black Forest Rule", PvPBiomeRule.Pvp, new ConfigDescription("Set up the pvp rule in Black Forest. Possible values: Pvp,Pve,Any."));
                pvpRuleInSwamp = config("2 - PvP Settings", "Biome 3 - Swamp Rule", PvPBiomeRule.Pvp, new ConfigDescription("Set up the pvp rule in Swamp. Possible values: Pvp,Pve,Any."));
                pvpRuleInMountain = config("2 - PvP Settings", "Biome 4 - Mountain Rule", PvPBiomeRule.Pvp, new ConfigDescription("Set up the pvp rule in Mountain. Possible values: Pvp,Pve,Any."));
                pvpRuleInPlains = config("2 - PvP Settings", "Biome 5 - Plains Rule", PvPBiomeRule.Pvp, new ConfigDescription("Set up the pvp rule in Plains. Possible values: Pvp,Pve,Any."));
                pvpRuleInMistlands = config("2 - PvP Settings", "Biome 6 - Mistlands Rule", PvPBiomeRule.Pvp, new ConfigDescription("Set up the pvp rule in Mistlands. Possible values: Pvp,Pve,Any."));
                pvpRuleInAshlands = config("2 - PvP Settings", "Biome 7 - Ashlands Rule", PvPBiomeRule.Pvp, new ConfigDescription("Set up the pvp rule in Ashlands. Possible values: Pvp,Pve,Any."));
                pvpRuleInDeepNorth = config("2 - PvP Settings", "Biome 8 - Deep North Rule", PvPBiomeRule.Pvp, new ConfigDescription("Set up the pvp rule in Deep North. Possible values: Pvp,Pve,Any."));
                pvpRuleInOcean = config("2 - PvP Settings", "Biome 9 - Ocean Rule", PvPBiomeRule.Pvp, new ConfigDescription("Set up the pvp rule in Ocean. Possible values: Pvp,Pve,Any."));

                positionAdminExempt = config("3 - Map Position", "Admin Exempt", Toggle.On, new ConfigDescription("If on, server admins can bypass the 'Position Always On' rule.")); 
                positionRuleInWards = config("3 - Map Position", "Position Rule In Wards", PositionSharingWardRule.Biome, new ConfigDescription("Set up the position sharing in wards, overriding biome rules if it's needed. Possible values: Pvp,Pve,Any,Biome"));
                positionRuleInMeadows = config("3 - Map Position", "Biome 1 - Meadows Rule", PositionSharingBiomeRule.Show, new ConfigDescription("Set up the position sharing rule in Meadows. Possible values: Hide,Show,Any."));
                positionRuleInBlackForest = config("3 - Map Position", "Biome 2 - Black Forest Rule", PositionSharingBiomeRule.Show, new ConfigDescription("Set up the position sharing in Black Forest. Possible values: Hide,Show,Any."));
                positionRuleInSwamp = config("3 - Map Position", "Biome 3 - Swamp Rule", PositionSharingBiomeRule.Show, new ConfigDescription("Set up the position sharing in Swamp. Possible values: Hide,Show,Any."));
                positionRuleInMountain = config("3 - Map Position", "Biome 4 - Mountain Rule", PositionSharingBiomeRule.Show, new ConfigDescription("Set up the position sharing in Mountain. Possible values: Hide,Show,Any."));
                positionRuleInPlains = config("3 - Map Position", "Biome 5 - Plains Rule", PositionSharingBiomeRule.Show, new ConfigDescription("Set up the position sharing in Plains. Possible values: Hide,Show,Any."));
                positionRuleInMistlands = config("3 - Map Position", "Biome 6 - Mistlands Rule", PositionSharingBiomeRule.Show, new ConfigDescription("Set up the position sharing in Mistlands. Possible values: Hide,Show,Any."));
                positionRuleInAshlands = config("3 - Map Position", "Biome 7 - Ashlands Rule", PositionSharingBiomeRule.Show, new ConfigDescription("Set up the position sharing in Ashlands. Possible values: Hide,Show,Any."));
                positionRuleInDeepNorth = config("3 - Map Position", "Biome 8 - Deep North Rule", PositionSharingBiomeRule.Show, new ConfigDescription("Set up the position sharing in Deep North. Possible values: Hide,Show,Any."));
                positionRuleInOcean = config("3 - Map Position", "Biome 9 - Ocean Rule", PositionSharingBiomeRule.Show, new ConfigDescription("Set up the position sharing in Ocean. Possible values: Hide,Show,Any."));

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
