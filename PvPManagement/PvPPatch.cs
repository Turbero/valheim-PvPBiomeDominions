using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using PvPBiomeDominions.Helpers;
using PvPBiomeDominions.Helpers.WardIsLove;
using UnityEngine.UI;

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
    
    [HarmonyPatch(typeof(Character), "RPC_Damage")]
    [HarmonyPriority(Priority.VeryHigh)]
    public class PvPDamageCheckWackyEpicMMOPatch
    {
        static bool Prefix(Character __instance, HitData hit)
        {
            if (!GameManager.isWackyEpicMMOSystemInstalled())
                return true;
            
            if (__instance  != null && __instance.GetType() == typeof(Player))
            {
                Character attacker = hit.GetAttacker();
                if (attacker != null && attacker.GetType() == typeof(Player))
                {
                    //Not autodamage
                    if (attacker.GetType() == typeof(Player) && (__instance as Player)?.GetPlayerName() == (attacker as Player)?.GetPlayerName())
                        return true;

                    
                    Player playerAttacker = attacker as Player;
                    Dictionary<string, string> attackerKnownTexts = (Dictionary<string, string>) GameManager.GetPrivateValue(playerAttacker, "m_knownTexts");
                    string attackerLevelStr = attackerKnownTexts["EpicMMOSystem_LevelSystem_Level"];
                    bool attackerHasLevel = int.TryParse(attackerLevelStr, out int attackerLevel);
                    
                    Player playerAttacked = __instance as Player;
                    Dictionary<string, string> attackedKnownTexts = (Dictionary<string, string>) GameManager.GetPrivateValue(playerAttacked, "m_knownTexts");
                    string attackedLevelStr = attackedKnownTexts["EpicMMOSystem_LevelSystem_Level"];
                    bool attackedHasLevel = int.TryParse(attackedLevelStr, out int attackedLevel);

                    if (attackerHasLevel && attackedHasLevel && (attackedLevel - attackerLevel) > ConfigurationFile.pvpWackyEpicMMOLevelDifferenceLimit.Value)
                        return false;
                }
            }

            return true;
        }
    }
    
    [HarmonyPatch(typeof(Minimap), "UpdatePlayerPins")]
    public static class Minimap_UpdatePlayerPins_Patch
    {
        static void Postfix(Minimap __instance)
        {
            Dictionary<string, ZNet.PlayerInfo> znetPlayerInfos = ZNet.instance.GetPlayerList()
                .GroupBy(p => p.m_name).ToDictionary(p => p.Key, p => p.First());

            List<Minimap.PinData> m_pins = (List<Minimap.PinData>) GameManager.GetPrivateValue(__instance, "m_pins");
            foreach (var pin in m_pins)
            {
                if (pin.m_type != Minimap.PinType.Player || pin.m_name == Player.m_localPlayer.GetPlayerName())
                    continue;

                // Get PlayerInfo
                ZDOID charID = znetPlayerInfos.GetValueSafe(pin.m_name).m_characterID;

                // Find PvP status
                bool isPVP = false;
                ZDO zdo = ZDOMan.instance.GetZDO(charID);
                if (zdo != null)
                    isPVP = zdo.GetBool("pvp");

                // Update sprint accordingly
                var img = pin.m_uiElement?.GetComponent<Image>();
                if (img == null)
                    continue;

                img.sprite = isPVP ? ImageManager.spriteIconVanillaImage : ImageManager.spriteBlueIconImage;
            }
        }
    }


}