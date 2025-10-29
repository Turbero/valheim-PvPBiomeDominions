using System.Linq;
using HarmonyLib;
using PvPBiomeDominions.PvPManagement.UI;
using UnityEngine;

namespace PvPBiomeDominions.PvPManagement
{
    [HarmonyPatch(typeof(Minimap), "Update")]
    public class MapPlayersListPanelPatch
    {
        private static bool created = false;
        private static float lastUpdateTime = 0f;

        private static PlayersListPanel panel;

        static void Postfix(Minimap __instance)
        {
            // Create components
            if (!created && __instance.m_largeRoot != null)
            {
                panel = new PlayersListPanel(__instance);
                created = true;
                
                if (!canUpdate(__instance))
                    return;
                panel.UpdateList(ZNet.instance.GetPlayerList().OrderBy(p => p.m_name).ToList());
            }

            // If map not opened, we don't refresh
            if (!canUpdate(__instance))
                return;

            // Only refresh when more than X seconds has passed from last time
            if (Time.time - lastUpdateTime >= ConfigurationFile.pvpMinimapPlayersListRefresh.Value)
            {
                lastUpdateTime = Time.time;
                panel.UpdateList(ZNet.instance.GetPlayerList().OrderBy(p => p.m_name).ToList());
            }
        }

        private static bool canUpdate(Minimap __instance)
        {
            return __instance.m_largeRoot != null && __instance.m_largeRoot.activeSelf && panel.panelRoot.activeSelf;
        }
    }
}