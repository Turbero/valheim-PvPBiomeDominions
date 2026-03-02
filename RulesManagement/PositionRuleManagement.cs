using System;
using System.Linq;
using TMPro;

namespace PvPBiomeDominions.RulesManagement
{
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

    public class PositionRuleManagement
    {
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
                case Heightmap.Biome.Meadows: return ConfigurationFile.positionRuleInMeadows.Value;
                case Heightmap.Biome.BlackForest: return ConfigurationFile.positionRuleInBlackForest.Value;
                case Heightmap.Biome.Swamp: return ConfigurationFile.positionRuleInSwamp.Value;
                case Heightmap.Biome.Mountain: return ConfigurationFile.positionRuleInMountain.Value;
                case Heightmap.Biome.Plains: return ConfigurationFile.positionRuleInPlains.Value;
                case Heightmap.Biome.Mistlands: return ConfigurationFile.positionRuleInMistlands.Value;
                case Heightmap.Biome.AshLands: return ConfigurationFile.positionRuleInAshlands.Value;
                case Heightmap.Biome.DeepNorth: return ConfigurationFile.positionRuleInDeepNorth.Value;
                case Heightmap.Biome.Ocean: return ConfigurationFile.positionRuleInOcean.Value;
                default: return getCustomBiomeRulePosition();
            }
        }

        private static PositionSharingBiomeRule getCustomBiomeRulePosition()
        {
            if (string.IsNullOrEmpty(ConfigurationFile.positionRuleInCustomBiomes.Value) || Minimap.instance == null)
                return PositionSharingBiomeRule.PlayerChoice;

            string currentCustomBiomeName = Minimap.instance.transform.Find("small/small_biome")?.GetComponent<TextMeshProUGUI>()?.text;
            if (currentCustomBiomeName == null) return PositionSharingBiomeRule.PlayerChoice;
            
            foreach (var entry in ConfigurationFile.positionRuleInCustomBiomes.Value.Split(';').ToList())
            {
                if (!entry.Contains(":")) continue;
                string[] config = entry.Split(':');
                
                string biomeName = config[0];
                if (biomeName.Equals(currentCustomBiomeName))
                {
                    bool done = Enum.TryParse(config[1], out PositionSharingBiomeRule positionBiomeRule);
                    Logger.Log($"{done} {positionBiomeRule}");
                    if (done)
                        return positionBiomeRule;
                    return PositionSharingBiomeRule.PlayerChoice;
                }
            }
            return PositionSharingBiomeRule.PlayerChoice;
        }
        
        
    }
}