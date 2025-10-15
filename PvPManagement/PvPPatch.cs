using System;
using HarmonyLib;
using PvPBiomeDominions.PvPManagement.Helpers.WardIsLove;

namespace PvPBiomeDominions.PvPManagement
{
    [HarmonyPatch]
    static class PlayerUpdatePatch
    {
        private static bool _insideWard;

        [HarmonyPatch(typeof(Player), "Update")]
        [HarmonyPatch(typeof(InventoryGui), "UpdateCharacterStats")]
        [HarmonyPostfix]
        public static void Postfix(Player __instance)
        {
            if (!ZNetScene.instance || !Game.instance | !Player.m_localPlayer || InventoryGui.instance) return;

            ConfigurationFile.PvPRule currentBiomeRule = ConfigurationFile.getCurrentBiomeRule();
            if (currentBiomeRule == ConfigurationFile.PvPRule.Free)
            {
                InventoryGui.instance.m_pvp.interactable = true;
                return;
            }
            
            try
            {
                _insideWard = WardIsLovePlugin.IsLoaded()
                    ? WardMonoscript.InsideWard(Player.m_localPlayer.transform.position)
                    : PrivateArea.InsideFactionArea(Player.m_localPlayer.transform.position, Character.Faction.Players);
                bool isInsideTerritory = Marketplace_API.IsInstalled() &&
                                         Marketplace_API.IsPointInsideTerritoryWithFlag(
                                             Player.m_localPlayer.transform.position,
                                             Marketplace_API.TerritoryFlags.PveOnly, out _, out _, out _);
                if (isInsideTerritory) return;
                if (_insideWard && ConfigurationFile.offPvPInWards.Value == ConfigurationFile.Toggle.On) return;

                InventoryGui.instance.m_pvp.interactable = false;
                PvPEnforcer(InventoryGui.instance, currentBiomeRule == ConfigurationFile.PvPRule.Pvp);
            }
            catch (Exception exception)
            {
                Logger.LogError($"There was an error in setting the PvP {exception}");
            }
        }
        
        private static void PvPEnforcer(InventoryGui invGUI, bool isPvPOn)
        {
            Player.m_localPlayer.SetPVP(isPvPOn);
            invGUI.m_pvp.isOn = isPvPOn;
        }
    }
}