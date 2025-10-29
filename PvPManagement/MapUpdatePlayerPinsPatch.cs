using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using PvPBiomeDominions.Helpers;
using UnityEngine.UI;

namespace PvPBiomeDominions.PvPManagement
{
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

                var img = pin.m_uiElement?.GetComponent<Image>();
                if (img == null)
                    continue;

                // Find PvP status with playerInfo and update sprite
                bool isPVP = GameManager.isInfoPVP(znetPlayerInfos.GetValueSafe(pin.m_name));
                img.sprite = isPVP ? ImageManager.spriteIconVanillaImage : ImageManager.spriteBlueIconImage;
            }
        }
    }
}