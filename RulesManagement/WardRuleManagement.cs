using System.Linq;
using TMPro;

namespace PvPBiomeDominions.RulesManagement
{
    public class WardRuleManagement
    {
        public static bool IsWardCreationAllowedInCurrentBiomeRule()
        {
            Logger.Log("IsWardCreationAllowedInCurrentBiomeRule");
            if (!EnvMan.instance) return true;
            
            Heightmap.Biome biome = EnvMan.instance.GetCurrentBiome();
            switch (biome)
            {
                case Heightmap.Biome.Meadows: return ConfigurationFile.wardCreationInMeadows.Value == ConfigurationFile.Toggle.On;
                case Heightmap.Biome.BlackForest: return ConfigurationFile.wardCreationInBlackForest.Value == ConfigurationFile.Toggle.On;
                case Heightmap.Biome.Swamp: return ConfigurationFile.wardCreationInSwamp.Value == ConfigurationFile.Toggle.On;
                case Heightmap.Biome.Mountain: return ConfigurationFile.wardCreationInMountain.Value == ConfigurationFile.Toggle.On;
                case Heightmap.Biome.Plains: return ConfigurationFile.wardCreationInPlains.Value == ConfigurationFile.Toggle.On;
                case Heightmap.Biome.Mistlands: return ConfigurationFile.wardCreationInMistlands.Value == ConfigurationFile.Toggle.On;
                case Heightmap.Biome.AshLands: return ConfigurationFile.wardCreationInAshlands.Value == ConfigurationFile.Toggle.On;
                case Heightmap.Biome.DeepNorth: return ConfigurationFile.wardCreationInDeepNorth.Value == ConfigurationFile.Toggle.On;
                case Heightmap.Biome.Ocean: return ConfigurationFile.wardCreationInOcean.Value == ConfigurationFile.Toggle.On;
                default: return getWardCreationInCustomBiome();
            }
        }
        
        private static bool getWardCreationInCustomBiome()
        {
            Logger.Log("getWardCreationInCustomBiome");
            if (string.IsNullOrEmpty(ConfigurationFile.wardCreationForbiddenInCustomBiomes.Value) || Minimap.instance == null)
                return true;
            
            string currentCustomBiomeName = Minimap.instance.transform.Find("small/small_biome")?.GetComponent<TextMeshProUGUI>()?.text;
            if (currentCustomBiomeName == null) return true;
            
            foreach (var customBiomeName in ConfigurationFile.wardCreationForbiddenInCustomBiomes.Value.Split(',').ToList())
            {
                if (customBiomeName.Equals(currentCustomBiomeName))
                    return false;
            }
            
            return true;
        }

        
    }
}