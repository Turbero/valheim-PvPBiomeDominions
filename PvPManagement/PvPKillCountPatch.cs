using System.Collections.Generic;
using HarmonyLib;
using PvPBiomeDominions.Helpers;
using PvPBiomeDominions.PositionManagement;

namespace PvPBiomeDominions.PvPManagement
{
    //FIXME
    //[HarmonyPatch(typeof(Character), nameof(Character.ApplyDamage))]
    public class PvPKillCountPatch
    {
        public static void Postfix(Character __instance, HitData hit)
        {
            Logger.Log("PvPKillCountPatch Postfix. Type: "+__instance.GetType());
            if (__instance.IsTamed()) return;
            if (__instance.GetHealth() > 0f) return;

            if (__instance  != null && __instance.GetType() == typeof(Player))
            {
                Logger.Log("Player killed detected.");
                Player playerKilled = __instance as Player;
                
                //Update knownTexts
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
                        Logger.Log("New key value: "+finalCount);
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
                
                //Refresh UI immediately if possible
                if (MinimapUpdatePatch.panel != null)
                    MinimapUpdatePatch.panel.UpdatePlayerKilledCount(playerKilled.GetPlayerName(), finalCount);
            }
        }
    }
}