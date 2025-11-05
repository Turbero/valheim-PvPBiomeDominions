using BepInEx.Configuration;
using BepInEx;
using System;
using System.IO;
using ServerSync;
using UnityEngine;

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
            PlayerChoose = 2
        }
        public enum PvPWardRule
        {
            Pve = 0,
            Pvp = 1,
            PlayerChoose = 2,
            FollowBiomeRule = 3
        }
        public enum PositionSharingBiomeRule
        {
            HidePlayer = 0,
            ShowPlayer = 1,
            PlayerChoice = 2
        }
        public enum PositionSharingWardRule
        {
            HidePlayer = 0,
            ShowPlayer = 1,
            PlayerChoice = 2,
            FollowBiomeRule = 3
        }
        
        private static ConfigEntry<bool> _serverConfigLocked = null;

        public static ConfigEntry<bool> debug;
        public static ConfigEntry<Vector2> mapPlayersListPosition;
        public static ConfigEntry<Vector2> mapPlayersListSize;
        public static ConfigEntry<bool> mapPinColoring;

        public static ConfigFile configFile;
        private static readonly string ConfigFileName = PvPBiomeDominions.GUID + ".cfg";
        private static readonly string ConfigFileFullPath = Paths.ConfigPath + Path.DirectorySeparatorChar + ConfigFileName;

        public static readonly ConfigSync ConfigSync = new ConfigSync(PvPBiomeDominions.GUID)
        {
            DisplayName = PvPBiomeDominions.NAME,
            CurrentVersion = PvPBiomeDominions.VERSION,
            MinimumRequiredVersion = PvPBiomeDominions.VERSION
        };
        
        //PVP Management
        public static ConfigEntry<Toggle> pvpAdminExempt;
        public static ConfigEntry<PvPWardRule> pvpRuleInWards;
        private static ConfigEntry<PvPBiomeRule> pvpRuleInMeadows;
        private static ConfigEntry<PvPBiomeRule> pvpRuleInBlackForest;
        private static ConfigEntry<PvPBiomeRule> pvpRuleInSwamp;
        private static ConfigEntry<PvPBiomeRule> pvpRuleInMountain;
        private static ConfigEntry<PvPBiomeRule> pvpRuleInPlains;
        private static ConfigEntry<PvPBiomeRule> pvpRuleInMistlands;
        private static ConfigEntry<PvPBiomeRule> pvpRuleInAshlands;
        private static ConfigEntry<PvPBiomeRule> pvpRuleInDeepNorth;
        private static ConfigEntry<PvPBiomeRule> pvpRuleInOcean;
        public static ConfigEntry<string> pvpTombstoneLootAlertMessage;
        public static ConfigEntry<string> pvpTombstoneDestroyAlertMessage;
        public static ConfigEntry<int> pvpMinimapPlayersListRefresh;

        //Position management
        public static ConfigEntry<Toggle> positionAdminExempt;
        public static ConfigEntry<PositionSharingWardRule> positionRuleInWards;
        private static ConfigEntry<PositionSharingBiomeRule> positionRuleInMeadows;
        private static ConfigEntry<PositionSharingBiomeRule> positionRuleInBlackForest;
        private static ConfigEntry<PositionSharingBiomeRule> positionRuleInSwamp;
        private static ConfigEntry<PositionSharingBiomeRule> positionRuleInMountain;
        private static ConfigEntry<PositionSharingBiomeRule> positionRuleInPlains;
        private static ConfigEntry<PositionSharingBiomeRule> positionRuleInMistlands;
        private static ConfigEntry<PositionSharingBiomeRule> positionRuleInAshlands;
        private static ConfigEntry<PositionSharingBiomeRule> positionRuleInDeepNorth;
        private static ConfigEntry<PositionSharingBiomeRule> positionRuleInOcean;
        
        //Other mods integration
        public static ConfigEntry<Toggle> pvpWackyEpicMMOLevelDifferenceLimitEnabled;
        public static ConfigEntry<int> pvpWackyEpicMMOLevelDifferenceLimitValue;
        
        //Translations
        public static ConfigEntry<string> playersListPanelButtonText;
        public static ConfigEntry<string> playersMapListTitle;

        public static PvPBiomeRule getCurrentBiomeRulePvPRule()
        {
            if (!EnvMan.instance) return PvPBiomeRule.PlayerChoose;
            
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

            return PvPBiomeRule.PlayerChoose;
        }
        
        public static PositionSharingBiomeRule getCurrentBiomeRulePosition()
        {
            if (!EnvMan.instance) return PositionSharingBiomeRule.PlayerChoice;
            return getBiomeRulePosition(EnvMan.instance.GetCurrentBiome());
        }
        
        public static PositionSharingBiomeRule getBiomeRulePosition(Heightmap.Biome biome)
        {
            if (!EnvMan.instance) return PositionSharingBiomeRule.PlayerChoice;
            
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

            return PositionSharingBiomeRule.PlayerChoice;
        }

        internal static void LoadConfig(BaseUnityPlugin plugin)
        {
            {
                configFile = plugin.Config;

                _serverConfigLocked = config("1 - General", "Lock Configuration", true, "If on, the configuration is locked and can be changed by server admins only.");
                _ = ConfigSync.AddLockingConfigEntry(_serverConfigLocked);

                debug = config("1 - General", "DebugMode", false, "Enabling/Disabling the debugging in the console (default = false)", false);
                mapPlayersListPosition = config("1 - General", "Map Players List Position", new Vector2(-602, 100), "Left corner position for the map players list", false);
                mapPlayersListSize = config("1 - General", "Map Players List Size", new Vector2(350, 620), "Left corner position for the map players list", false);
                mapPinColoring = config("1 - General", "Map Pins PvP Coloring", true, "Enable/disable the pins coloring in the player maps according to their pvp status", false);
                
                pvpAdminExempt = config("2 - PvP Settings", "Admin Exempt", Toggle.On, new ConfigDescription("If on, server admins can bypass the pvp biomes rules."));
                pvpRuleInWards = config("2 - PvP Settings", "PvP Rule In Wards (override biome)", PvPWardRule.FollowBiomeRule, new ConfigDescription("Set up the pvp rule inside wards, overriding biome rules if it's needed. Possible values: Pvp,Pve,PlayerChoice,FollowBiomeRule"));
                pvpRuleInMeadows = config("2 - PvP Settings", "Biome 1 - Meadows Rule", PvPBiomeRule.Pvp, new ConfigDescription("Set up the pvp rule in Meadows. Possible values: Pvp,Pve,PlayerChoice."));
                pvpRuleInBlackForest = config("2 - PvP Settings", "Biome 2 - Black Forest Rule", PvPBiomeRule.Pvp, new ConfigDescription("Set up the pvp rule in Black Forest. Possible values: Pvp,Pve,PlayerChoice."));
                pvpRuleInSwamp = config("2 - PvP Settings", "Biome 3 - Swamp Rule", PvPBiomeRule.Pvp, new ConfigDescription("Set up the pvp rule in Swamp. Possible values: Pvp,Pve,PlayerChoice."));
                pvpRuleInMountain = config("2 - PvP Settings", "Biome 4 - Mountain Rule", PvPBiomeRule.Pvp, new ConfigDescription("Set up the pvp rule in Mountain. Possible values: Pvp,Pve,PlayerChoice."));
                pvpRuleInPlains = config("2 - PvP Settings", "Biome 5 - Plains Rule", PvPBiomeRule.Pvp, new ConfigDescription("Set up the pvp rule in Plains. Possible values: Pvp,Pve,PlayerChoice."));
                pvpRuleInMistlands = config("2 - PvP Settings", "Biome 6 - Mistlands Rule", PvPBiomeRule.Pvp, new ConfigDescription("Set up the pvp rule in Mistlands. Possible values: Pvp,Pve,PlayerChoice."));
                pvpRuleInAshlands = config("2 - PvP Settings", "Biome 7 - Ashlands Rule", PvPBiomeRule.Pvp, new ConfigDescription("Set up the pvp rule in Ashlands. Possible values: Pvp,Pve,PlayerChoice."));
                pvpRuleInDeepNorth = config("2 - PvP Settings", "Biome 8 - Deep North Rule", PvPBiomeRule.Pvp, new ConfigDescription("Set up the pvp rule in Deep North. Possible values: Pvp,Pve,PlayerChoice."));
                pvpRuleInOcean = config("2 - PvP Settings", "Biome 9 - Ocean Rule", PvPBiomeRule.Pvp, new ConfigDescription("Set up the pvp rule in Ocean. Possible values: Pvp,Pve,PlayerChoice."));
                pvpTombstoneLootAlertMessage = config("2 - PvP Settings", "Tombstone Looting Alert Message", "<color=red>{0} is looting your tombstone!</color>", new ConfigDescription("Shows alert on tombstone owner screen when being looted by a different player"));
                pvpTombstoneDestroyAlertMessage = config("2 - PvP Settings", "Tombstone Destroy Alert Message", "<color=red>{0} sacked and destroyed your tombstone!</color>", new ConfigDescription("Shows alert on tombstone owner screen when destroyed by a different player"));
                pvpMinimapPlayersListRefresh = config("2 - PvP Settings", "Map Players List Refresh", 10, new ConfigDescription("Time in seconds to refresh the players list in the map."));
                    
                positionAdminExempt = config("3 - Map Position", "Admin Exempt", Toggle.On, new ConfigDescription("If on, server admins can bypass the 'Position Always On' rule.")); 
                positionRuleInWards = config("3 - Map Position", "Position Rule In Wards (override biome)", PositionSharingWardRule.FollowBiomeRule, new ConfigDescription("Set up the position sharing in wards, overriding biome rules if it's needed. Possible values: HidePlayer,ShowPlayer,PlayerChoice,FollowBiomeRule"));
                positionRuleInMeadows = config("3 - Map Position", "Biome 1 - Meadows Rule", PositionSharingBiomeRule.ShowPlayer, new ConfigDescription("Set up the position sharing rule in Meadows. Possible values: HidePlayer,ShowPlayer,PlayerChoice."));
                positionRuleInBlackForest = config("3 - Map Position", "Biome 2 - Black Forest Rule", PositionSharingBiomeRule.ShowPlayer, new ConfigDescription("Set up the position sharing in Black Forest. Possible values: HidePlayer,ShowPlayer,PlayerChoice."));
                positionRuleInSwamp = config("3 - Map Position", "Biome 3 - Swamp Rule", PositionSharingBiomeRule.ShowPlayer, new ConfigDescription("Set up the position sharing in Swamp. Possible values: HidePlayer,ShowPlayer,PlayerChoice."));
                positionRuleInMountain = config("3 - Map Position", "Biome 4 - Mountain Rule", PositionSharingBiomeRule.ShowPlayer, new ConfigDescription("Set up the position sharing in Mountain. Possible values: HidePlayer,ShowPlayer,PlayerChoice."));
                positionRuleInPlains = config("3 - Map Position", "Biome 5 - Plains Rule", PositionSharingBiomeRule.ShowPlayer, new ConfigDescription("Set up the position sharing in Plains. Possible values: HidePlayer,ShowPlayer,PlayerChoice."));
                positionRuleInMistlands = config("3 - Map Position", "Biome 6 - Mistlands Rule", PositionSharingBiomeRule.ShowPlayer, new ConfigDescription("Set up the position sharing in Mistlands. Possible values: HidePlayer,ShowPlayer,PlayerChoice."));
                positionRuleInAshlands = config("3 - Map Position", "Biome 7 - Ashlands Rule", PositionSharingBiomeRule.ShowPlayer, new ConfigDescription("Set up the position sharing in Ashlands. Possible values: HidePlayer,ShowPlayer,PlayerChoice."));
                positionRuleInDeepNorth = config("3 - Map Position", "Biome 8 - Deep North Rule", PositionSharingBiomeRule.ShowPlayer, new ConfigDescription("Set up the position sharing in Deep North. Possible values: HidePlayer,ShowPlayer,PlayerChoice."));
                positionRuleInOcean = config("3 - Map Position", "Biome 9 - Ocean Rule", PositionSharingBiomeRule.ShowPlayer, new ConfigDescription("Set up the position sharing in Ocean. Possible values: HidePlayer,ShowPlayer,PlayerChoice."));
                
                pvpWackyEpicMMOLevelDifferenceLimitEnabled = config("4 - Mods integration", "Max Level Difference to damage in PvP areas - Activation", Toggle.Off, new ConfigDescription("Activate the limits the difference of levels between players to damage each other in pvp areas."));
                pvpWackyEpicMMOLevelDifferenceLimitValue = config("4 - Mods integration", "Max Level Difference to damage in PvP areas - Value", 100, new ConfigDescription("Limits the difference of levels between players to damage each other in pvp areas (default = 100)."));
                
                playersListPanelButtonText = config("5 - Translations", "Players List Panel Button Text", "Show/Hide list", new ConfigDescription("Button name used to show/hide the players panel list in the minimap."));
                playersMapListTitle = config("5 - Translations", "Players Map List Title", "Players", new ConfigDescription("Title of the map players list with connected count."));

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
