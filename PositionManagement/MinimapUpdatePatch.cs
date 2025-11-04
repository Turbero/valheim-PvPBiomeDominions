using System.Linq;
using System.Reflection;
using HarmonyLib;
using PvPBiomeDominions.Helpers;
using PvPBiomeDominions.Helpers.WardIsLove;
using PvPBiomeDominions.PositionManagement.UI;
using UnityEngine;

namespace PvPBiomeDominions.PositionManagement
{
    [HarmonyPatch(typeof(Minimap), "Update")]
    public static class MinimapUpdatePatch
    {
        private static bool created = false;
        private static float lastUpdateTime = 0f;
        
        public static PlayersListPanel panel;
        public static void Postfix(Minimap __instance)
        {
            if (!__instance) return;
            if (ConfigurationFile.positionAdminExempt.Value == ConfigurationFile.Toggle.On && ConfigurationFile.ConfigSync.IsAdmin)
                return;

            if (Player.m_localPlayer == null) return;
            
            // First ward rule
            bool isInsideWard = WardIsLovePlugin.IsInsideWard();
            var wardPositionRule = ConfigurationFile.positionRuleInWards.Value;
            if (isInsideWard && wardPositionRule != ConfigurationFile.PositionSharingWardRule.FollowBiomeRule)
            {
                if (wardPositionRule == ConfigurationFile.PositionSharingWardRule.PlayerChoice)
                    return;
                
                __instance.m_publicPosition.isOn = wardPositionRule == ConfigurationFile.PositionSharingWardRule.ShowPlayer;
                return;
            }

            // Then biome rule
            var fieldInfo = __instance.GetType().GetField("m_biome", BindingFlags.Instance | BindingFlags.NonPublic);
            if (fieldInfo == null)
                return;
            
            var mBiome = fieldInfo.GetValue(__instance);
            if (mBiome == null)
                return;
            
            //Heightmap.Biome minimapBiome = (Heightmap.Biome) mBiome;
            Heightmap.Biome minimapBiome = Player.m_localPlayer.GetCurrentBiome();
            ConfigurationFile.PositionSharingBiomeRule biomePositionBiomeRule = ConfigurationFile.getBiomeRulePosition(minimapBiome);
            if (biomePositionBiomeRule == ConfigurationFile.PositionSharingBiomeRule.PlayerChoice)
                return;
            
            __instance.m_publicPosition.isOn = biomePositionBiomeRule == ConfigurationFile.PositionSharingBiomeRule.ShowPlayer;
            
            if (__instance.m_publicPosition.isOn)
                ImageManager.UpdateMapSelectorIcon();

            HandlePlayersListPanel(__instance);

            // Refresh config for sync issues
            if (__instance.m_largeRoot.activeSelf && Time.time - lastUpdateTime >= ConfigurationFile.pvpMinimapPlayersListRefresh.Value)
            {
                lastUpdateTime = Time.time;
                ConfigurationFile.configFile.Reload();
            }
        }

        private static void HandlePlayersListPanel(Minimap __instance)
        {
            if (__instance.m_largeRoot == null)
                return;
            
            // Create components
            if (!created || panel == null || panel.panelRoot == null)
            {
                Logger.Log("HandlePlayersListPanel - First time");
                panel = new PlayersListPanel(__instance);
                created = true;
                
                if (!canUpdate(__instance))
                    return;
                panel.RefreshContent(ZNet.instance.GetPlayerList().OrderBy(p => p.m_name).ToList(), true);
                
                return;
            }

            // If map not opened, we don't refresh
            if (!canUpdate(__instance))
                return;

            // Only refresh when more than X seconds has passed from last time
            if (Time.time - lastUpdateTime >= ConfigurationFile.pvpMinimapPlayersListRefresh.Value)
            {
                lastUpdateTime = Time.time;
                panel.RefreshContent(ZNet.instance.GetPlayerList().OrderBy(p => p.m_name).ToList(), false);
            }
        }

        private static bool canUpdate(Minimap __instance)
        {
            return __instance.m_largeRoot != null && __instance.m_largeRoot.activeSelf &&
                   panel != null && panel.panelRoot != null && panel.panelRoot.activeSelf;
        }
    }
}