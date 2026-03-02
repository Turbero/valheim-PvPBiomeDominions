using System;
using System.Linq;
using TMPro;

namespace PvPBiomeDominions.RulesManagement
{
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

    public class PvPRuleManagement
    {
        public static PvPBiomeRule getCurrentBiomeRulePvPRule()
        {
            if (!EnvMan.instance) return PvPBiomeRule.PlayerChoose;
            
            Heightmap.Biome biome = EnvMan.instance.GetCurrentBiome();
            switch (biome)
            {
                case Heightmap.Biome.Meadows: return ConfigurationFile.pvpRuleInMeadows.Value;
                case Heightmap.Biome.BlackForest: return ConfigurationFile.pvpRuleInBlackForest.Value;
                case Heightmap.Biome.Swamp: return ConfigurationFile.pvpRuleInSwamp.Value;
                case Heightmap.Biome.Mountain: return ConfigurationFile.pvpRuleInMountain.Value;
                case Heightmap.Biome.Plains: return ConfigurationFile.pvpRuleInPlains.Value;
                case Heightmap.Biome.Mistlands: return ConfigurationFile.pvpRuleInMistlands.Value;
                case Heightmap.Biome.AshLands: return ConfigurationFile.pvpRuleInAshlands.Value;
                case Heightmap.Biome.DeepNorth: return ConfigurationFile.pvpRuleInDeepNorth.Value;
                case Heightmap.Biome.Ocean: return ConfigurationFile.pvpRuleInOcean.Value;
                default: return GetCurrentCustomBiome();
            }
        }

        private static PvPBiomeRule GetCurrentCustomBiome()
        {
            if (string.IsNullOrEmpty(ConfigurationFile.pvpRuleInCustomBiomes.Value) || Minimap.instance == null)
                return PvPBiomeRule.PlayerChoose;
            
            string currentCustomBiomeName = Minimap.instance.transform.Find("small/small_biome")?.GetComponent<TextMeshProUGUI>()?.text;
            if (currentCustomBiomeName == null) return PvPBiomeRule.PlayerChoose;
            
            foreach (var entry in ConfigurationFile.pvpRuleInCustomBiomes.Value.Split(';').ToList())
            {
                if (!entry.Contains(":")) continue;
                string[] config = entry.Split(':');
                
                string biomeName = config[0];
                if (biomeName.Equals(currentCustomBiomeName))
                {
                    bool done = Enum.TryParse(config[1], out PvPBiomeRule customBiomeRule);
                    if (done)
                        return customBiomeRule;
                    return PvPBiomeRule.PlayerChoose;
                }
            }
            return PvPBiomeRule.PlayerChoose;
        }
        
        
    }
}