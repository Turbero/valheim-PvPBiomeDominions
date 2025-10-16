using HarmonyLib;
using PvPBiomeDominions.Helpers.WardIsLove;

namespace PvPBiomeDominions.PvPManagement
{
    [HarmonyPatch]
    public class PlayerUpdatePatch
    {

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
            
            bool isInsideTerritory = Marketplace_API.IsInstalled() && Marketplace_API.IsPointInsideTerritoryWithFlag(Player.m_localPlayer.transform.position, Marketplace_API.TerritoryFlags.PveOnly, out _, out _, out _);
            if (isInsideTerritory) return;
            
            // Apply insideWard rule first
            bool isInsideWard = WardIsLovePlugin.IsInsideWard();
            var wardPvPRule = ConfigurationFile.pvpRuleInWards.Value;
            if (isInsideWard && wardPvPRule != ConfigurationFile.PvPWardRule.FollowBiomeRule) {
                if (wardPvPRule == ConfigurationFile.PvPWardRule.PlayerChoose)
                {
                    InventoryGui.instance.m_pvp.interactable = true;
                    return;
                }
                InventoryGui.instance.m_pvp.interactable = false;
                SetupPvP(InventoryGui.instance, wardPvPRule == ConfigurationFile.PvPWardRule.Pvp);
                return;
            }
            
            // Then check biome rule
            ConfigurationFile.PvPBiomeRule currentBiomeBiomeRule = ConfigurationFile.getCurrentBiomeRulePvPRule();
            if (currentBiomeBiomeRule == ConfigurationFile.PvPBiomeRule.PlayerChoose)
            {
                InventoryGui.instance.m_pvp.interactable = true;
                return;
            }
            InventoryGui.instance.m_pvp.interactable = false;

            SetupPvP(InventoryGui.instance, currentBiomeBiomeRule == ConfigurationFile.PvPBiomeRule.Pvp);
        }
        
        private static void SetupPvP(InventoryGui invGUI, bool isPvPOn)
        {
            Player.m_localPlayer.SetPVP(isPvPOn);
            invGUI.m_pvp.isOn = isPvPOn;
        }
    }
}