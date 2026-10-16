using System.Collections.Generic;
using HarmonyLib;
using PvPBiomeDominions.Helpers.WardIsLove;

namespace PvPBiomeDominions.ForbiddenManagement
{
    [HarmonyPatch(typeof(Humanoid), "EquipItem")]
    public class ForbiddenEquipPatch
    {
        public static bool Prefix(Humanoid __instance, ItemDrop.ItemData item, bool triggerEquipEffects)
        {
            if (!ZNetScene.instance) return true;
            if (Game.instance && !Player.m_localPlayer) return true;
            
            //Admins exempt rule
            if (ConfigurationFile.forbiddenEquipmentAdminExempt.Value == ConfigurationFile.Toggle.On && ConfigurationFile.ConfigSync.IsAdmin)
                return true;
            
            // Apply insideWard/territory rule first
            bool isInsideWard = WardIsLovePlugin.IsInsideWard();
            bool isInsideTerritory = Marketplace_API.IsInstalled() && Marketplace_API.IsPointInsideTerritoryWithFlag(Player.m_localPlayer.transform.position, Marketplace_API.TerritoryFlags.PveOnly, out _, out _, out _);
            if (isInsideWard || isInsideTerritory)
            {
                var forbiddenEquipmentWardIgnoreRule = ConfigurationFile.forbiddenEquipmentIgnoreInWards.Value;
                if (forbiddenEquipmentWardIgnoreRule == ConfigurationFile.Toggle.Off)
                {
                    Effects.scareEffect();
                    return false;
                }

                return true;
            }
            
            // Then check biome rule
            List<string> currentBiomeForbiddenEquipmentList = ForbiddenRulesManagement.getForbiddenEquipmentInCurrentBiomeRule();
            if (currentBiomeForbiddenEquipmentList.Contains(item.m_dropPrefab.name))
            {
                Effects.scareEffect();
                return false;
            }
            
            return true;
        }
    }
}