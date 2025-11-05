using System.Collections.Generic;
using HarmonyLib;
using PvPBiomeDominions.Helpers;
using PvPBiomeDominions.PositionManagement;

namespace PvPBiomeDominions.PvPManagement
{
    [HarmonyPatch(typeof(Character), "OnDeath")]
    [HarmonyPriority(Priority.VeryLow)]
    public class PvPKillCountPatch
    {
        static void Postfix(Character __instance)
        {
            if (__instance  != null && __instance.GetType() == typeof(Player))
            {
                Player playerAttacker = Player.m_localPlayer;
                Player playerKilled = __instance as Player;
                
                //TODO Update knownTexts
                var dicKnownTexts = (Dictionary<string, string>)GameManager.GetPrivateValue(Player.m_localPlayer, "m_knownTexts");
                string prefixKills = GameManager.PREFIX_KILLS;
                var knownText = prefixKills + playerKilled.GetPlayerName();
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
                    }
                    else
                    {
                        //Clean up ugly value and start over
                        dicKnownTexts.Remove(knownText);
                        dicKnownTexts.Add(knownText, "1");
                        Logger.Log("UpdatePlayerRelevantInfo - Add new knownText: "+knownText+" with value=1");
                    }
                }
                else
                {
                    dicKnownTexts.Add(knownText, "1");
                    Logger.Log("UpdatePlayerRelevantInfo - Add new knownText: "+knownText+" with value=1");
                }
                
                //TODO Refresh UI immediately if possible
                MinimapUpdatePatch.panel.UpdatePlayerKilledCount(playerKilled.GetPlayerName(), finalCount);
            }
        }
    }
}