using System.Collections.Generic;
using System.Linq;
using Groups;
using HarmonyLib;
using PvPBiomeDominions.Helpers;
using PvPBiomeDominions.PositionManagement;
using PvPBiomeDominions.PositionManagement.UI;
using UnityEngine.UI;

namespace PvPBiomeDominions.PvPManagement
{
    [HarmonyPatch(typeof(Minimap), "UpdatePlayerPins")]
    [HarmonyPriority(Priority.Last)]
    public static class Minimap_UpdatePlayerPins_Patch
    {
        static void Postfix(Minimap __instance)
        {
            if (!ConfigurationFile.mapPinColoring.Value || MinimapUpdatePatch.panel == null) return;
                
            Dictionary<string, ZNet.PlayerInfo> znetPlayerInfos = ZNet.instance.GetPlayerList()
                .GroupBy(p => p.m_name).ToDictionary(p => p.Key, p => p.First());

            List<Minimap.PinData> m_pins = (List<Minimap.PinData>) GameManager.GetPrivateValue(__instance, "m_pins");
            List<Minimap.PinData> playerPins = m_pins.Where(p =>
                p.m_type == Minimap.PinType.Player).ToList();

            List<PlayerReference> groupPlayers = new List<PlayerReference>();
            if (Groups.API.IsLoaded())
            {
                groupPlayers.AddRange(Groups.API.GroupPlayers());
            }
            foreach (var pin in playerPins)
            {
                // Don't touch in next cases:
                if (pin.m_type != Minimap.PinType.Player ||                       // not a player pin 
                    string.IsNullOrEmpty(pin.m_name) ||                           // empty name
                    pin.m_name == Player.m_localPlayer.GetPlayerName() ||         // not local player
                    !znetPlayerInfos.Keys.Contains(pin.m_name) ||                 // not a player name
                    groupPlayers.FindIndex(pRef => pRef.name == pin.m_name) >= 0) // player is part of the localPlayer group
                    continue;
                
                var img = pin.m_uiElement?.GetComponent<Image>();
                if (img == null)
                    continue;

                // Find PvP status (first cache then playerInfo) and update sprite
                PlayerEntry playerEntry = MinimapUpdatePatch.panel.cachedPlayerEntries.Find(cpe => cpe.name.Equals(pin.m_name));
                bool isPVP = playerEntry != null ? playerEntry.isPvP : GameManager.isInfoPVP(znetPlayerInfos.GetValueSafe(pin.m_name));
                img.sprite = isPVP ? ImageManager.spriteIconVanillaImage : ImageManager.spriteBlueIconImage;
            }
        }
    }
}