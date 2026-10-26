using System.Collections.Generic;
using HarmonyLib;
using PvPBiomeDominions.Helpers.WardIsLove;
using PvPBiomeDominions.RulesManagement;

namespace PvPBiomeDominions.ForbiddenManagement
{
    [HarmonyPatch(typeof(Humanoid), "EquipItem")]
    public class ForbiddenEquipPatch
    {
        public static bool Prefix(Humanoid __instance, ItemDrop.ItemData item, bool triggerEquipEffects)
        {
            if (!ZNetScene.instance) return true;
            if (Game.instance && !Player.m_localPlayer) return true;
            
            //Only for players
            if (__instance is not Player) return true;
            
            //Admins exempt rule
            if (ConfigurationFile.forbiddenEquipmentAdminExempt.Value == ConfigurationFile.Toggle.On && ConfigurationFile.ConfigSync.IsAdmin)
                return true;
            
            // Apply insideWard/territory rule first
            bool isInsideWard = WardIsLovePlugin.IsInsideWard();
            bool isInsideTerritory = Marketplace_API.IsInstalled() && Marketplace_API.IsPointInsideTerritoryWithFlag(Player.m_localPlayer.transform.position, Marketplace_API.TerritoryFlags.PveOnly, out _, out _, out _);
            if (isInsideWard || isInsideTerritory)
            {
                var forbiddenEquipmentWardIgnoreRule = ConfigurationFile.forbiddenEquipmentIgnoreInWards.Value;
                if (forbiddenEquipmentWardIgnoreRule == ConfigurationFile.Toggle.On)
                    return true;
            }
            
            // Then check biome rule
            List<string> currentBiomeForbiddenEquipmentList = ForbiddenRuleManagement.getForbiddenEquipmentInCurrentBiomeRule();
            if (currentBiomeForbiddenEquipmentList.Contains(item.m_dropPrefab.name))
            {
                Effects.scareEffect();
                Player.m_localPlayer.Message(MessageHud.MessageType.Center, "$npc_dvergrmage_random_private_area_alarm3");
                return false;
            }
            
            return true;
        }
    }
    
    [HarmonyPatch(typeof(EnvMan), "UpdateEnvironment")]
    public static class BiomeChangePatch
    {
        private static Heightmap.Biome previousBiome;

        public static void Postfix(long sec, BiomeSector biome)
        {
            if (biome.Biome != previousBiome)
            {
                Logger.Log($"Biome changed: {previousBiome} -> {biome.Biome}");
                previousBiome = biome.Biome;

                if (Player.m_localPlayer == null) return;
                
                //Force to remove forbidden equipment in new biome
                List<ItemDrop.ItemData> equipped = Player.m_localPlayer.GetInventory().GetEquippedItems();
                foreach (var equippedItem in equipped)
                {
                    if (!ForbiddenEquipPatch.Prefix(Player.m_localPlayer, equippedItem, false))
                        Player.m_localPlayer.UnequipItem(equippedItem, false);
                }
            }
        }
    }
}