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
                UpdateInsideWard();

                if (ConfigurationFile.positionAdminExempt.Value == ConfigurationFile.Toggle.On && ConfigurationFile.ConfigSync.IsAdmin)
                    return;

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
                
                __instance.m_publicPosition.isOn = calculatePositionOn(biomePositionRule);
            }
        }

        [HarmonyPatch(typeof(Player), nameof(Player.OnSpawned))]
        static class PlayerOnSpawnedPatch
        {
            static void Postfix(Player __instance)
            {
                UpdateInsideWard();

                if (ConfigurationFile.positionAdminExempt.Value == ConfigurationFile.Toggle.On && ConfigurationFile.ConfigSync.IsAdmin)
                    return;

                bool shouldSet = calculatePositionOn(ConfigurationFile.getCurrentBiomeRulePosition());
                ZNet.instance.SetPublicReferencePosition(shouldSet);
            }
        }
        
        private static void UpdateInsideWard()
        {
            if (Player.m_localPlayer != null)
            {
                isInsideWard = WardIsLovePlugin.IsLoaded()
                    ? WardMonoscript.InsideWard(Player.m_localPlayer.transform.position)
                    : PrivateArea.InsideFactionArea(Player.m_localPlayer.transform.position, Character.Faction.Players);
            }
        }
        
        private static bool calculatePositionOn(ConfigurationFile.PositionSharingRule positionRule)
        {
            if (ConfigurationFile.positionOffInWards.Value == ConfigurationFile.Toggle.On)
            {
                if (isInsideWard)
                {
                    return false;
                }

                return positionRule == ConfigurationFile.PositionSharingRule.Show;
            }

            return positionRule == ConfigurationFile.PositionSharingRule.Show;
        }


    }
}