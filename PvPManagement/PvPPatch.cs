using HarmonyLib;
using PvPBiomeDominions.Helpers.WardIsLove;

namespace PvPBiomeDominions.PvPManagement
{
    [HarmonyPatch]
    static class PlayerUpdatePatch
    {
        private static bool isInsideWard;

        [HarmonyPatch(typeof(Player), "Update")]
        [HarmonyPatch(typeof(InventoryGui), "UpdateCharacterStats")]
        [HarmonyPostfix]
        public static void Postfix(Player __instance)
        {
            if (!ZNetScene.instance) return;
            if (Game.instance && !Player.m_localPlayer) return;
            if (!InventoryGui.instance) return;
            
            if (ConfigurationFile.pvpAdminExempt.Value == ConfigurationFile.Toggle.On && ConfigurationFile.ConfigSync.IsAdmin)
                return;
            
            // Apply insideWard rule first
            isInsideWard = WardIsLovePlugin.IsInsideWard();
            bool isInsideTerritory = Marketplace_API.IsInstalled() && Marketplace_API.IsPointInsideTerritoryWithFlag(Player.m_localPlayer.transform.position, Marketplace_API.TerritoryFlags.PveOnly, out _, out _, out _);
            if (isInsideTerritory) return;
            if (isInsideWard) {
                var wardPvPRule = ConfigurationFile.pvpRuleInWards.Value;
                if (wardPvPRule == ConfigurationFile.PvPRule.Any)
                {
                    InventoryGui.instance.m_pvp.interactable = true;
                    return;
                }
                InventoryGui.instance.m_pvp.interactable = false;
                
                SetupPvP(InventoryGui.instance, wardPvPRule == ConfigurationFile.PvPRule.Pvp);
                return;
            }
            
            // Then check biome rule
            ConfigurationFile.PvPRule currentBiomeRule = ConfigurationFile.getCurrentBiomeRulePvPRule();
            if (currentBiomeRule == ConfigurationFile.PvPRule.Any)
            {
                InventoryGui.instance.m_pvp.interactable = true;
                return;
            }
            InventoryGui.instance.m_pvp.interactable = false;

            SetupPvP(InventoryGui.instance, currentBiomeRule == ConfigurationFile.PvPRule.Pvp);
        }
        
        private static void SetupPvP(InventoryGui invGUI, bool isPvPOn)
        {
            Player.m_localPlayer.SetPVP(isPvPOn);
            invGUI.m_pvp.isOn = isPvPOn;
        }
    }
}