using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using PvPBiomeDominions.Helpers;
using PvPBiomeDominions.PositionManagement;

namespace PvPBiomeDominions.PvPManagement
{
    [HarmonyPatch(typeof(Player), nameof(Player.GetKnownTexts))]
    public class FixCompendium
    {
        public static void Postfix(ref List<KeyValuePair<string, string>> __result)
        {
            __result = __result.Where(p => !p.Key.StartsWith(PvPBiomeDominions.GUID)).ToList();
        }
    }
    
    [HarmonyPatch(typeof(Character), nameof(Character.ApplyDamage))]
    public class PvPKillCountPatch
    {
        [HarmonyPostfix]
        public static void ApplyReceivedDamageToCharacter(Character __instance, HitData hit)
        {
            Logger.Log("[ApplyReceivedDamageToCharacter] Postfix. Damage received to character type: "+__instance.GetType());
            if (__instance.IsTamed()) return;
            if (__instance.GetHealth() > 0f) return;
            
            if (__instance != Player.m_localPlayer) return;
            Logger.Log("Player killed detected!");
            Player playerKilled = __instance as Player;

            Character attacker = hit.GetAttacker();
            if (attacker.gameObject.name != "Player(Clone)") return;
            
            Player attackerPlayer = attacker as Player;
            Logger.Log($"Player killed by Player {attackerPlayer.GetPlayerName()} detected!");
            
            //TODO Test UserId to send to killer
            Logger.Log($"Trying RPC with {attackerPlayer.GetZDOID().UserID} and {playerKilled.GetPlayerName()}");
            ZRoutedRpc.instance.InvokeRoutedRPC(attackerPlayer.GetZDOID().UserID, "RPC_AddKillToKiller", playerKilled.GetPlayerName());
        }

        public static void RPC_AddKillToKiller(long sender, string killedPlayerName)
        {
            Logger.Log("[RPC_AddKillToKiller] RPC called.");
            
            //Update knownTexts
            var dicKnownTexts = (Dictionary<string, string>)GameManager.GetPrivateValue(Player.m_localPlayer, "m_knownTexts");
            string prefixKills = GameManager.PREFIX_KILLS;
            var knownText = prefixKills + killedPlayerName;
            int finalCount = 1;
            if (dicKnownTexts.ContainsKey(knownText))
            {
                string value = dicKnownTexts.GetValueSafe(knownText);
                bool isInt = int.TryParse(value, out int valueInt);
                if (isInt)
                {
                    //Replace value with +1
                    dicKnownTexts.Remove(knownText);
                    dicKnownTexts.Add(knownText, ""+(valueInt+1));
                    finalCount = valueInt + 1;
                    Logger.LogInfo("New key value: "+finalCount);
                }
                else
                {
                    //Clean up ugly value and start over
                    dicKnownTexts.Remove(knownText);
                    dicKnownTexts.Add(knownText, finalCount.ToString());
                    Logger.Log("[RPC_AddKillToKiller] Add new knownText: "+knownText+" with value=1");
                }
            }
            else
            {
                dicKnownTexts.Add(knownText, "1");
                Logger.Log("[RPC_AddKillToKiller] Add new knownText: "+knownText+" with value=1");
            }
                
            //Refresh UI immediately if possible
            if (MinimapUpdatePatch.panel != null)
                MinimapUpdatePatch.panel.UpdatePlayerKillsCount(killedPlayerName, finalCount);
        }
    }
}