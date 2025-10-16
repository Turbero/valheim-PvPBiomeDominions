using System;
using BepInEx.Bootstrap;
using BepInEx.Configuration;

namespace PvPBiomeDominions.Helpers.WardIsLove
{
    public class WardIsLovePlugin : ModCompat
    {
        private const string GUID = "Azumatt.WardIsLove";
        private static readonly System.Version MinVersion = new(2, 3, 3);

        public static bool IsInsideWard()
        {
            if (Player.m_localPlayer != null)
            {
                return IsLoaded()
                    ? WardMonoscript.InsideWard(Player.m_localPlayer.transform.position)
                    : PrivateArea.InsideFactionArea(Player.m_localPlayer.transform.position, Character.Faction.Players);
            }

            return false;
        }
        
        private static Type ClassType()
        {
            return Type.GetType("WardIsLove.WardIsLovePlugin, WardIsLove");
        }

        private static bool IsLoaded()
        {
            return Chainloader.PluginInfos.ContainsKey(GUID) &&
                   Chainloader.PluginInfos[GUID].Metadata.Version >= MinVersion;
        }

        public static ConfigEntry<bool> WardEnabled()
        {
            return GetField<ConfigEntry<bool>>(ClassType(), null, "WardEnabled");
        }
    }
}