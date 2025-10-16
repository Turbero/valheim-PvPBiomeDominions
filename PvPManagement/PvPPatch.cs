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
            
            ConfigurationFile.PvPRule currentBiomeRule = ConfigurationFile.getCurrentBiomeRulePvPRule();
            if (currentBiomeRule == ConfigurationFile.PvPRule.Any)
            {
                InventoryGui.instance.m_pvp.interactable = true;
                return;
            }
            InventoryGui.instance.m_pvp.interactable = false;
            
            isInsideWard = WardIsLovePlugin.IsLoaded()
                ? WardMonoscript.InsideWard(Player.m_localPlayer.transform.position)
                : PrivateArea.InsideFactionArea(Player.m_localPlayer.transform.position, Character.Faction.Players);
            bool isInsideTerritory = Marketplace_API.IsInstalled() && Marketplace_API.IsPointInsideTerritoryWithFlag(Player.m_localPlayer.transform.position, Marketplace_API.TerritoryFlags.PveOnly, out _, out _, out _);
            if (isInsideTerritory) return;
            if (isInsideWard && ConfigurationFile.pvpOffInWards.Value == ConfigurationFile.Toggle.On)
            {
                InventoryGui.instance.m_pvp.interactable = true;
                return;
            }
            
            if (currentBiomeRule == ConfigurationFile.PvPRule.Pve)
            {
                SetupPvP(InventoryGui.instance, false);
                return;
            }
            
            SetupPvP(InventoryGui.instance, true);
        }
        
        private static void SetupPvP(InventoryGui invGUI, bool isPvPOn)
        {
            Player.m_localPlayer.SetPVP(isPvPOn);
            invGUI.m_pvp.isOn = isPvPOn;
        }
    }
}