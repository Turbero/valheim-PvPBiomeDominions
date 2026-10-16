using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;

namespace PvPBiomeDominions.ForbiddenManagement
{
    public class ForbiddenRulesManagement
    {
        public static List<string> getForbiddenEquipmentInCurrentBiomeRule()
        {
            List<string> list = new List<string>();
            Logger.Log("getForbiddenEquipmentInCurrentBiomeRule");
            if (!EnvMan.instance) return list;
            
            Heightmap.Biome biome = EnvMan.instance.GetCurrentBiome();
            switch (biome)
            {
                case Heightmap.Biome.Meadows: return ConfigurationFile.forbiddenEquipmentRuleInMeadows.Value.Split(',').ToList();
                case Heightmap.Biome.BlackForest: return ConfigurationFile.forbiddenEquipmentRuleInBlackForest.Value.Split(',').ToList();
                case Heightmap.Biome.Swamp: return ConfigurationFile.forbiddenEquipmentRuleInSwamp.Value.Split(',').ToList();
                case Heightmap.Biome.Mountain: return ConfigurationFile.forbiddenEquipmentRuleInMountain.Value.Split(',').ToList();
                case Heightmap.Biome.Plains: return ConfigurationFile.forbiddenEquipmentRuleInPlains.Value.Split(',').ToList();
                case Heightmap.Biome.Mistlands: return ConfigurationFile.forbiddenEquipmentRuleInMistlands.Value.Split(',').ToList();
                case Heightmap.Biome.AshLands: return ConfigurationFile.forbiddenEquipmentRuleInAshlands.Value.Split(',').ToList();
                case Heightmap.Biome.DeepNorth: return ConfigurationFile.forbiddenEquipmentRuleInDeepNorth.Value.Split(',').ToList();
                case Heightmap.Biome.Ocean: return ConfigurationFile.forbiddenEquipmentRuleInOcean.Value.Split(',').ToList();
                default: return getForbiddenEquipmentRuleInCustomBiome();
            }
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
            
            Heightmap.Biome biome = EnvMan.instance.GetCurrentBiome();
            switch (biome)
            {
                case Heightmap.Biome.Meadows: return ConfigurationFile.forbiddenAttackRuleInMeadows.Value.Split(',').ToList();
                case Heightmap.Biome.BlackForest: return ConfigurationFile.forbiddenAttackRuleInBlackForest.Value.Split(',').ToList();
                case Heightmap.Biome.Swamp: return ConfigurationFile.forbiddenAttackRuleInSwamp.Value.Split(',').ToList();
                case Heightmap.Biome.Mountain: return ConfigurationFile.forbiddenAttackRuleInMountain.Value.Split(',').ToList();
                case Heightmap.Biome.Plains: return ConfigurationFile.forbiddenAttackRuleInPlains.Value.Split(',').ToList();
                case Heightmap.Biome.Mistlands: return ConfigurationFile.forbiddenAttackRuleInMistlands.Value.Split(',').ToList();
                case Heightmap.Biome.AshLands: return ConfigurationFile.forbiddenAttackRuleInAshlands.Value.Split(',').ToList();
                case Heightmap.Biome.DeepNorth: return ConfigurationFile.forbiddenAttackRuleInDeepNorth.Value.Split(',').ToList();
                case Heightmap.Biome.Ocean: return ConfigurationFile.forbiddenAttackRuleInOcean.Value.Split(',').ToList();
                default: return getForbiddenAttackRuleInCustomBiome();
            }
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