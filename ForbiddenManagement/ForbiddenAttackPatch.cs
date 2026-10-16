using System.Collections.Generic;
using HarmonyLib;
using PvPBiomeDominions.Helpers.WardIsLove;

namespace PvPBiomeDominions.ForbiddenManagement
{
    //[HarmonyPatch(typeof(Player), "PlayerAttackInput")]
    [HarmonyPatch(typeof(Humanoid), "StartAttack")]
    public class ForbiddenAttackPatch
    {
        //public static bool Prefix(Player __instance, float dt)
        public static bool Prefix(Humanoid __instance, Character target, bool secondaryAttack)
        {
            if (!ZNetScene.instance) return true;
            if (Game.instance && !Player.m_localPlayer) return true;
            
            //Only for players
            if (__instance is not Player) return true;
            
            //Admins exempt rule
            if (ConfigurationFile.forbiddenAttackAdminExempt.Value == ConfigurationFile.Toggle.On && ConfigurationFile.ConfigSync.IsAdmin)
                return true;
            
            // Apply insideWard/territory rule first
            bool isInsideWard = WardIsLovePlugin.IsInsideWard();
            bool isInsideTerritory = Marketplace_API.IsInstalled() && Marketplace_API.IsPointInsideTerritoryWithFlag(Player.m_localPlayer.transform.position, Marketplace_API.TerritoryFlags.PveOnly, out _, out _, out _);
            if (isInsideWard || isInsideTerritory)
            {
                var forbiddenAttackWardIgnoreRule = ConfigurationFile.forbiddenAttackIgnoreInWards.Value;
                if (forbiddenAttackWardIgnoreRule == ConfigurationFile.Toggle.Off)
                {
                    Effects.scareEffect();
                    return false;
                }

                return true;
            }

            // Then check biome rule
            List<string> currentBiomeForbiddenAttackList = ForbiddenAttackManagement.getForbiddenAttackInCurrentBiomeRule();
            if (currentBiomeForbiddenAttackList.Contains(__instance.GetCurrentWeapon().m_dropPrefab.name))
            {
                Effects.scareEffect();
                return false;
            }

            return true;
        }
    }
}