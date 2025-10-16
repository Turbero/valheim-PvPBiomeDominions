using System.Reflection;
using HarmonyLib;
using PvPBiomeDominions.Helpers.WardIsLove;

namespace PvPBiomeDominions.PositionManagement
{
    public class PositionPatch
    {
        
        private static bool isInsideWard;
        
        [HarmonyPatch(typeof(ZNet), nameof(ZNet.SetPublicReferencePosition))]
        public static class PublicReferencePositionPatch
        {
            private static void Postfix(ref bool pub, ref bool ___m_publicReferencePosition)
            {
                if (ConfigurationFile.positionAdminExempt.Value == ConfigurationFile.Toggle.On && ConfigurationFile.ConfigSync.IsAdmin)
                    return;
                
                // Ward rule first
                isInsideWard = WardIsLovePlugin.IsInsideWard();
                if (isInsideWard)
                {
                    ConfigurationFile.PositionSharingRule wardPositionRule = ConfigurationFile.positionRuleInWards.Value;
                    if (wardPositionRule == ConfigurationFile.PositionSharingRule.Any)
                        return;
                    
                    ___m_publicReferencePosition = wardPositionRule == ConfigurationFile.PositionSharingRule.Show;
                    return;
                }
                
                // Then biome rule
                ConfigurationFile.PositionSharingRule biomePositionRule = ConfigurationFile.getCurrentBiomeRulePosition();
                if (biomePositionRule == ConfigurationFile.PositionSharingRule.Any)
                    return;
                
                ___m_publicReferencePosition = biomePositionRule == ConfigurationFile.PositionSharingRule.Show;
            }
        }

        [HarmonyPatch(typeof(Minimap), "Update")]
        static class MinimapUpdatePatch
        {
            static void Postfix(Minimap __instance)
            {
                if (!__instance) return;
                if (ConfigurationFile.positionAdminExempt.Value == ConfigurationFile.Toggle.On && ConfigurationFile.ConfigSync.IsAdmin)
                    return;

                // First ward rule
                isInsideWard = WardIsLovePlugin.IsInsideWard();
                if (isInsideWard)
                {
                    ConfigurationFile.PositionSharingRule wardPositionRule = ConfigurationFile.positionRuleInWards.Value;
                    if (wardPositionRule == ConfigurationFile.PositionSharingRule.Any)
                        return;
                    
                    __instance.m_publicPosition.isOn = wardPositionRule == ConfigurationFile.PositionSharingRule.Show;
                    return;
                }

                // Then biome rule
                var fieldInfo = __instance.GetType().GetField("m_biome", BindingFlags.Instance | BindingFlags.NonPublic);
                if (fieldInfo == null)
                    return;
                
                var mBiome = fieldInfo.GetValue(__instance);
                if (mBiome == null)
                    return;
                
                Heightmap.Biome minimapBiome = (Heightmap.Biome) mBiome;
                ConfigurationFile.PositionSharingRule biomePositionRule = ConfigurationFile.getBiomeRulePosition(minimapBiome);
                if (biomePositionRule == ConfigurationFile.PositionSharingRule.Any)
                    return;
                
                __instance.m_publicPosition.isOn = biomePositionRule == ConfigurationFile.PositionSharingRule.Show;
            }
        }

        [HarmonyPatch(typeof(Player), nameof(Player.OnSpawned))]
        static class PlayerOnSpawnedPatch
        {
            static void Postfix(Player __instance)
            {
                if (ConfigurationFile.positionAdminExempt.Value == ConfigurationFile.Toggle.On && ConfigurationFile.ConfigSync.IsAdmin)
                    return;

                // First ward rule
                isInsideWard = WardIsLovePlugin.IsInsideWard();
                if (isInsideWard)
                {
                    ConfigurationFile.PositionSharingRule wardPositionRule = ConfigurationFile.positionRuleInWards.Value;
                    if (wardPositionRule == ConfigurationFile.PositionSharingRule.Any)
                        return;
                    
                    ZNet.instance.SetPublicReferencePosition(wardPositionRule == ConfigurationFile.PositionSharingRule.Show);
                    return;
                }

                // Then biome rule
                ConfigurationFile.PositionSharingRule biomePositionRule = ConfigurationFile.getCurrentBiomeRulePosition();
                if (biomePositionRule == ConfigurationFile.PositionSharingRule.Any)
                    return;
                
                ZNet.instance.SetPublicReferencePosition(biomePositionRule == ConfigurationFile.PositionSharingRule.Show);
            }
        }
    }
}