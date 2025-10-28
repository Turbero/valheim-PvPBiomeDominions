using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using PvPBiomeDominions.Helpers;
using UnityEngine.UI;

namespace PvPBiomeDominions.PvPManagement
{
    public class MapPlayersListPatch
    {
        [HarmonyPatch(typeof(Minimap), "Awake")]
        public class MiniMap_Awake_Patch
        {
            public static void Postfix(Minimap __instance)
            {
                //TODO Create list inside map
                
            }
        }
        
        [HarmonyPatch(typeof(Minimap), "Update")]
        public class MiniMap_Update_Patch
        {
            public static void Postfix(Minimap __instance)
            {
                //TODO Update players list
            }
        }
    }
    
    
    
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

                // Get PlayerInfo
                ZDOID charID = znetPlayerInfos.GetValueSafe(pin.m_name).m_characterID;

                // Find PvP status
                bool isPVP = false;
                ZDO zdo = ZDOMan.instance.GetZDO(charID);
                if (zdo != null)
                    isPVP = zdo.GetBool("pvp");

                // Update sprint accordingly
                var img = pin.m_uiElement?.GetComponent<Image>();
                if (img == null)
                    continue;

                img.sprite = isPVP ? ImageManager.spriteIconVanillaImage : ImageManager.spriteBlueIconImage;
            }
        }
    }
}