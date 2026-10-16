using HarmonyLib;

namespace PvPBiomeDominions.Others
{
    [HarmonyPatch(typeof(Ship), "UpdateWaterForce")]
    public class Ship_WaterImpactDamage_Patch
    {
        private static void Prefix(Ship __instance, float depth, float time)
        {
            if (ConfigurationFile.waterImpactDamage.Value >= 0)
                __instance.m_waterImpactDamage = ConfigurationFile.waterImpactDamage.Value;
            else
                __instance.m_waterImpactDamage = 10f; //vanilla default value
        }
    }
}