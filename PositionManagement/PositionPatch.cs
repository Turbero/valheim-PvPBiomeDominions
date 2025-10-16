using System.Reflection;
using HarmonyLib;
using PvPBiomeDominions.Helpers.WardIsLove;

namespace PvPBiomeDominions.PositionManagement
{
    [HarmonyPatch(typeof(ZNet), nameof(ZNet.SetPublicReferencePosition))]
    public class PublicReferencePositionPatch
    {
        public static void Postfix(ref bool pub, ref bool ___m_publicReferencePosition)
        {
            if (ConfigurationFile.positionAdminExempt.Value == ConfigurationFile.Toggle.On && ConfigurationFile.ConfigSync.IsAdmin)
                return;
            
            // Ward rule first
            bool isInsideWard = WardIsLovePlugin.IsInsideWard();
            var wardPositionRule = ConfigurationFile.positionRuleInWards.Value;
            if (isInsideWard && wardPositionRule != ConfigurationFile.PositionSharingWardRule.Biome)
            {
                if (wardPositionRule == ConfigurationFile.PositionSharingWardRule.Any)
                    return;
                
                ___m_publicReferencePosition = wardPositionRule == ConfigurationFile.PositionSharingWardRule.Show;
                return;
            }
            
            // Then biome rule
            ConfigurationFile.PositionSharingBiomeRule biomePositionBiomeRule = ConfigurationFile.getCurrentBiomeRulePosition();
            if (biomePositionBiomeRule == ConfigurationFile.PositionSharingBiomeRule.Any)
                return;
            
            ___m_publicReferencePosition = biomePositionBiomeRule == ConfigurationFile.PositionSharingBiomeRule.Show;
        }
    }

    [HarmonyPatch(typeof(Minimap), "Update")]
    public class MinimapUpdatePatch
    {
        public static void Postfix(Minimap __instance)
        {
            if (!__instance) return;
            if (ConfigurationFile.positionAdminExempt.Value == ConfigurationFile.Toggle.On && ConfigurationFile.ConfigSync.IsAdmin)
                return;

            // First ward rule
            bool isInsideWard = WardIsLovePlugin.IsInsideWard();
            var wardPositionRule = ConfigurationFile.positionRuleInWards.Value;
            if (isInsideWard && wardPositionRule != ConfigurationFile.PositionSharingWardRule.Biome)
            {
                if (wardPositionRule == ConfigurationFile.PositionSharingWardRule.Any)
                    return;
                
                __instance.m_publicPosition.isOn = wardPositionRule == ConfigurationFile.PositionSharingWardRule.Show;
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
            ConfigurationFile.PositionSharingBiomeRule biomePositionBiomeRule = ConfigurationFile.getBiomeRulePosition(minimapBiome);
            if (biomePositionBiomeRule == ConfigurationFile.PositionSharingBiomeRule.Any)
                return;
            
            __instance.m_publicPosition.isOn = biomePositionBiomeRule == ConfigurationFile.PositionSharingBiomeRule.Show;
        }
    }

    [HarmonyPatch(typeof(Player), nameof(Player.OnSpawned))]
    public class PlayerOnSpawnedPatch
    {
        public static void Postfix(Player __instance)
        {
            if (ConfigurationFile.positionAdminExempt.Value == ConfigurationFile.Toggle.On && ConfigurationFile.ConfigSync.IsAdmin)
                return;

            // First ward rule
            bool isInsideWard = WardIsLovePlugin.IsInsideWard();
            var wardPositionRule = ConfigurationFile.positionRuleInWards.Value;
            if (isInsideWard && wardPositionRule != ConfigurationFile.PositionSharingWardRule.Biome)
            {
                if (wardPositionRule == ConfigurationFile.PositionSharingWardRule.Any)
                    return;
                
                ZNet.instance.SetPublicReferencePosition(wardPositionRule == ConfigurationFile.PositionSharingWardRule.Show);
                return;
            }

            // Then biome rule
            ConfigurationFile.PositionSharingBiomeRule biomePositionBiomeRule = ConfigurationFile.getCurrentBiomeRulePosition();
            if (biomePositionBiomeRule == ConfigurationFile.PositionSharingBiomeRule.Any)
                return;
            
            ZNet.instance.SetPublicReferencePosition(biomePositionBiomeRule == ConfigurationFile.PositionSharingBiomeRule.Show);
        }
    }
}