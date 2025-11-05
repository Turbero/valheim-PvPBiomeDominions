using System.Collections.Generic;
using HarmonyLib;
using PvPBiomeDominions.Helpers;

namespace PvPBiomeDominions.PvPManagement
{
    [HarmonyPatch(typeof(Character), "Damage")]
    [HarmonyPriority(Priority.VeryHigh)]
    public class PvPDamageCheckWackyEpicMMOPatch
    {
        static bool Prefix(HitData hit, Character __instance)
        {
            if (!EpicMMOSystem_API.IsLoaded() || ConfigurationFile.pvpWackyEpicMMOLevelDifferenceLimitEnabled.Value == ConfigurationFile.Toggle.Off)
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

                    if (attackerHasLevel && attackedHasLevel && (attackedLevel - attackerLevel) > ConfigurationFile.pvpWackyEpicMMOLevelDifferenceLimitValue.Value)
                        return false;
                }
            }

            return true;
        }
    }
}