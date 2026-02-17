using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using PvPBiomeDominions.Helpers;
using PvPBiomeDominions.PositionManagement;
using UnityEngine;

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
    

    [HarmonyPatch(typeof(Player), nameof(Player.OnSpawned))]
    public class SE_PvPSpawnProtectionPatch
    {
        public static void Postfix(Player __instance)
        {
            if (ConfigurationFile.waitingTimeAfterDyingToFightPlayersAgain.Value > 0)
            {
                Logger.Log("Adding PvP Buff Protection to "+__instance.GetPlayerName());
                var se = ScriptableObject.CreateInstance<SE_PvPSpawnImmunity>();
                se.m_ttl = ConfigurationFile.waitingTimeAfterDyingToFightPlayersAgain.Value * 60;

                __instance.GetSEMan().AddStatusEffect(se);
            }
        }
    }
    
    [HarmonyPatch(typeof(Character), nameof(Character.ApplyDamage))]
    public class PvPKillCountPatch
    {
        [HarmonyPrefix]
        public static bool CheckPvPProtectionBuff(Character __instance, HitData hit)
        {
            Logger.Log("[CheckPvPProtectionBuff] Prefix. Damage received to character type: " + __instance.GetType());
            //Buff PvP protection Check
            if (__instance != null && __instance is Player)
            {
                Character attacker = hit.GetAttacker();
                if (attacker is Player attackerPlayer)
                {
                    Logger.Log("[CheckPvPProtectionBuff] Prefix. Player vs Player detected");
                    if (__instance.GetSEMan().HaveStatusEffect(SE_PvPSpawnImmunity.HASH))
                    {
                        Logger.Log("Player that received hit has PvP protection buff and cannot be damaged by players yet!");
                        return false;
                    }

                    if (attackerPlayer!.GetSEMan().HaveStatusEffect(SE_PvPSpawnImmunity.HASH))
                    {
                        Logger.Log("Player that attacks has PvP protection buff and cannot damage players yet!");
                        return false;
                    }
                }
            }
            return true;
        }
        
        [HarmonyPostfix]
        public static void ApplyReceivedDamageToCharacter(Character __instance, HitData hit)
        {
            Logger.Log("[ApplyReceivedDamageToCharacter] Postfix. Damage received to character type: " + __instance.GetType());
            if (__instance.IsTamed()) return;
            if (__instance.GetHealth() > 0f) return;
            
            if (__instance != Player.m_localPlayer) return;
            Logger.Log("Player killed detected!");
            Player playerKilled = __instance as Player;

            if (hit == null || hit.GetAttacker() == null)
            {
                Logger.Log("Unknown death (/die command?)");
                return;
            }
            Character attacker = hit.GetAttacker();
            if (attacker.gameObject.name != "Player(Clone)") return;

            Player attackerPlayer = attacker as Player;
            var playerNameToFind = attackerPlayer.GetPlayerName();
            Logger.Log($"Player killed by Player {playerNameToFind} detected!");

            //Update knownTexts
            int finalCount = updateLocalPlayerKnownText(GameManager.PREFIX_KILLEDBY + playerNameToFind, "PvPKillCountPatch");

            //Refresh UI immediately if possible
            if (MinimapUpdatePatch.panel != null)
                MinimapUpdatePatch.panel.UpdatePlayerKilledByCount(playerNameToFind, finalCount);

            //Send to killer to update his/her kills count
            Logger.Log($"Trying RPC with {attackerPlayer.GetZDOID().UserID} and {playerKilled.GetPlayerName()}");
            ZRoutedRpc.instance.InvokeRoutedRPC(attackerPlayer.GetZDOID().UserID, "RPC_AddKillToKiller", playerKilled.GetPlayerName());
        }

        public static void RPC_AddKillToKiller(long sender, string killedPlayerName)
        {
            Logger.Log("[RPC_AddKillToKiller] RPC called.");

            //Update knownTexts
            int finalCount = updateLocalPlayerKnownText(GameManager.PREFIX_KILLS + killedPlayerName, "RPC_AddKillToKiller");

            //Refresh UI immediately if possible
            if (MinimapUpdatePatch.panel != null)
                MinimapUpdatePatch.panel.UpdatePlayerKillsCount(killedPlayerName, finalCount);
        }

        private static int updateLocalPlayerKnownText(string knownTextToUpdate, string callerMethod)
        {
            var dicKnownTexts = (Dictionary<string, string>)GameManager.GetPrivateValue(Player.m_localPlayer, "m_knownTexts");
            int finalCount = 1;
            if (dicKnownTexts.ContainsKey(knownTextToUpdate))
            {
                string value = dicKnownTexts.GetValueSafe(knownTextToUpdate);
                bool isInt = int.TryParse(value, out int valueInt);
                if (isInt)
                {
                    //Replace value with +1
                    dicKnownTexts.Remove(knownTextToUpdate);
                    dicKnownTexts.Add(knownTextToUpdate, "" + (valueInt + 1));
                    finalCount = valueInt + 1;
                    Logger.Log($"[{callerMethod}] New key value: " + finalCount);
                }
                else
                {
                    //Clean up ugly value and start over
                    dicKnownTexts.Remove(knownTextToUpdate);
                    dicKnownTexts.Add(knownTextToUpdate, finalCount.ToString());
                    Logger.Log($"[{callerMethod}] Add new knownText: " + knownTextToUpdate + " with value=1");
                }
            }
            else
            {
                dicKnownTexts.Add(knownTextToUpdate, "1");
                Logger.Log($"[{callerMethod}] Add new knownText: " + knownTextToUpdate + " with value=1");
            }

            return finalCount;
        }
    }
}