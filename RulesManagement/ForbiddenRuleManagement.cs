using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;

namespace PvPBiomeDominions.RulesManagement
{
    public class ForbiddenRuleManagement
    {
        public static List<string> getForbiddenEquipmentInCurrentBiomeRule()
        {
            List<string> list = new List<string>();
            Logger.Log("getForbiddenEquipmentInCurrentBiomeRule");
            if (!EnvMan.instance) return list;
            
            List<string> fullList = ConfigurationFile.forbiddenEquipmentRuleInAllBiomes.Value.Split(',').ToList();
            Heightmap.Biome biome = EnvMan.instance.GetCurrentBiome();
            switch (biome)
            {
                case Heightmap.Biome.Meadows:
                    fullList.AddRange(ConfigurationFile.forbiddenEquipmentRuleInMeadows.Value.Split(',').ToList()); break;
                case Heightmap.Biome.BlackForest:
                    fullList.AddRange(ConfigurationFile.forbiddenEquipmentRuleInBlackForest.Value.Split(',').ToList()); break;
                case Heightmap.Biome.Swamp:
                    fullList.AddRange(ConfigurationFile.forbiddenEquipmentRuleInSwamp.Value.Split(',').ToList()); break;
                case Heightmap.Biome.Mountain:
                    fullList.AddRange(ConfigurationFile.forbiddenEquipmentRuleInMountain.Value.Split(',').ToList()); break;
                case Heightmap.Biome.Plains:
                    fullList.AddRange(ConfigurationFile.forbiddenEquipmentRuleInPlains.Value.Split(',').ToList()); break;
                case Heightmap.Biome.Mistlands:
                    fullList.AddRange(ConfigurationFile.forbiddenEquipmentRuleInMistlands.Value.Split(',').ToList()); break;
                case Heightmap.Biome.AshLands:
                    fullList.AddRange(ConfigurationFile.forbiddenEquipmentRuleInAshlands.Value.Split(',').ToList()); break;
                case Heightmap.Biome.DeepNorth:
                    fullList.AddRange(ConfigurationFile.forbiddenEquipmentRuleInDeepNorth.Value.Split(',').ToList()); break;
                case Heightmap.Biome.Ocean:
                    fullList.AddRange(ConfigurationFile.forbiddenEquipmentRuleInOcean.Value.Split(',').ToList()); break;
                default: 
                    fullList.AddRange(getForbiddenEquipmentRuleInCustomBiome()); break;
            }

            return fullList;
        }
        
        private static List<string> getForbiddenEquipmentRuleInCustomBiome()
        {
            List<string> list = new List<string>();
            Logger.Log("getForbiddenEquipmentInCustomBiome");
            if (string.IsNullOrEmpty(ConfigurationFile.forbiddenEquipmentInCustomBiomes.Value) || Minimap.instance == null)
                return list;
            
            string currentCustomBiomeName = Minimap.instance.transform.Find("small/small_biome")?.GetComponent<TextMeshProUGUI>()?.text;
            if (currentCustomBiomeName == null) return list;

            try
            {
                foreach (var customBiomeInfo in ConfigurationFile.forbiddenEquipmentInCustomBiomes.Value
                             .Split(';').ToList())
                {
                    string[] customBiomeInfoParts = customBiomeInfo.Split(':');
                    string customBiomeName = customBiomeInfoParts[0];
                    if (customBiomeName.Equals(currentCustomBiomeName))
                        list.AddRange(customBiomeInfoParts[1].Split(',').ToList());
                }
            }
            catch (Exception e)
            {
                Logger.LogError("Error while parsing the forbiddenEquipmentInCustomBiomes value. Skipping... "+e.Message);
            }

            return list;
        }
    }
    
    public class ForbiddenAttackManagement
    {
        public static List<string> getForbiddenAttackInCurrentBiomeRule()
        {
            List<string> list = new List<string>();
            Logger.Log("getForbiddenAttackInCurrentBiomeRule");
            if (!EnvMan.instance) return list;
            
            List<string> fullList = ConfigurationFile.forbiddenAttackRuleInAllBiomes.Value.Split(',').ToList();
            Heightmap.Biome biome = EnvMan.instance.GetCurrentBiome();
            switch (biome)
            {
                case Heightmap.Biome.Meadows:
                    fullList.AddRange(ConfigurationFile.forbiddenAttackRuleInMeadows.Value.Split(',').ToList()); break;
                case Heightmap.Biome.BlackForest:
                    fullList.AddRange(ConfigurationFile.forbiddenAttackRuleInBlackForest.Value.Split(',').ToList()); break;
                case Heightmap.Biome.Swamp:
                    fullList.AddRange(ConfigurationFile.forbiddenAttackRuleInSwamp.Value.Split(',').ToList()); break;
                case Heightmap.Biome.Mountain:
                    fullList.AddRange(ConfigurationFile.forbiddenAttackRuleInMountain.Value.Split(',').ToList()); break;
                case Heightmap.Biome.Plains:
                    fullList.AddRange(ConfigurationFile.forbiddenAttackRuleInPlains.Value.Split(',').ToList()); break;
                case Heightmap.Biome.Mistlands:
                    fullList.AddRange(ConfigurationFile.forbiddenAttackRuleInMistlands.Value.Split(',').ToList()); break;
                case Heightmap.Biome.AshLands:
                    fullList.AddRange(ConfigurationFile.forbiddenAttackRuleInAshlands.Value.Split(',').ToList()); break;
                case Heightmap.Biome.DeepNorth:
                    fullList.AddRange(ConfigurationFile.forbiddenAttackRuleInDeepNorth.Value.Split(',').ToList()); break;
                case Heightmap.Biome.Ocean:
                    fullList.AddRange(ConfigurationFile.forbiddenAttackRuleInOcean.Value.Split(',').ToList()); break;
                default: 
                    fullList.AddRange(getForbiddenAttackRuleInCustomBiome()); break;
            }

            return fullList;
        }
        
        private static List<string> getForbiddenAttackRuleInCustomBiome()
        {
            List<string> list = new List<string>();
            Logger.Log("getForbiddenAttackInCustomBiome");
            if (string.IsNullOrEmpty(ConfigurationFile.forbiddenAttackInCustomBiomes.Value) || Minimap.instance == null)
                return list;
            
            string currentCustomBiomeName = Minimap.instance.transform.Find("small/small_biome")?.GetComponent<TextMeshProUGUI>()?.text;
            if (currentCustomBiomeName == null) return list;

            try
            {
                foreach (var customBiomeInfo in ConfigurationFile.forbiddenAttackInCustomBiomes.Value
                             .Split(';').ToList())
                {
                    string[] customBiomeInfoParts = customBiomeInfo.Split(':');
                    string customBiomeName = customBiomeInfoParts[0];
                    if (customBiomeName.Equals(currentCustomBiomeName))
                        list.AddRange(customBiomeInfoParts[1].Split(',').ToList());
                }
            }
            catch (Exception e)
            {
                Logger.LogError("Error while parsing the forbiddenAttackInCustomBiomes value. Skipping... "+e.Message);
            }

            return list;
        }
    }

    public class Effects
    {
        public static void scareEffect()
        {
            ScareEffect effect = ConfigurationFile.scareEffect.Value;
            if (effect != ScareEffect.None)
            {
                SEMan seMan = Player.m_localPlayer.GetSEMan();
                if (seMan?.GetStatusEffect(effect.ToString().GetHashCode()) == null)
                {
                    StatusEffect se = seMan?.AddStatusEffect(effect.ToString().GetHashCode(), resetTime: false);
                    if (se != null) se.m_ttl = ConfigurationFile.scareEffectDuration.Value;
                }
            }
        }
    }
}