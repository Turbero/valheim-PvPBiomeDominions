using System;
using System.Collections.Generic;
using System.Reflection;
using Groups;
using HarmonyLib;
using TMPro;
using UnityEngine;

namespace PvPBiomeDominions.Helpers
{
    public class GameManager
    {
        
        private static readonly Dictionary<string, Sprite> cachedSprites = new Dictionary<string, Sprite>();
        private static readonly Dictionary<string, TMP_FontAsset> cachedFonts = new Dictionary<string, TMP_FontAsset>();

        public static object GetPrivateValue(object obj, string name, BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.NonPublic)
        {
            return obj.GetType().GetField(name, bindingAttr)?.GetValue(obj);
        }

        public static bool isWackyEpicMMOSystemInstalled()
        {
            return EpicMMOSystem_API.IsLoaded();
        }

        public static bool isGroupsModInstalled()
        {
            return Groups.API.IsLoaded();
        }

        public static bool isInfoPVP(ZNet.PlayerInfo info)
        {
            ZDOID charID = info.m_characterID;
            
            // Find PvP status
            ZDO zdo = ZDOMan.instance.GetZDO(charID);
            if (zdo != null)
            {
                bool isPvP = zdo.GetBool("pvp", true); // TODO we say pvp by default for now even if it doesn't exist to use red default icon
                return isPvP;
            }
            Logger.Log($"no zdo found for {info.m_name}");
            return false;
        }
        public static Sprite getSprite(String name)
        {
            if (!cachedSprites.ContainsKey(name))
            {
                Logger.Log($"Finding {name} sprite...");
                var allSprites = Resources.FindObjectsOfTypeAll<Sprite>();
                for (var i = 0; i < allSprites.Length; i++)
                {
                    var sprite = allSprites[i];
                    if (sprite.name == name)
                    {
                        Logger.Log($"{name} sprite found.");
                        cachedSprites.Add(name, sprite);
                        return sprite;
                    }
                }
                Logger.Log($"{name} sprite NOT found.");
                return null;
            } else
            {
                return cachedSprites.GetValueSafe(name);
            }
        }

        public static TMP_FontAsset getFontAsset(String name)
        {
            if (!cachedFonts.ContainsKey(name))
            {
                Logger.Log($"Finding {name} font...");
                var allFonts = Resources.FindObjectsOfTypeAll<TMP_FontAsset>();
                for (var i = 0; i < allFonts.Length; i++)
                {
                    var font = allFonts[i];
                    if (font.name == name)
                    {
                        Logger.Log($"{name} font found.");
                        cachedFonts.Add(name, font);
                        return font;
                    }
                }
                Logger.Log($"{name} font NOT found.");
                return null;
            }
            else
            {
                return cachedFonts.GetValueSafe(name);
            }
        }

        public static List<PlayerReference> GetGroupPlayers()
        {
            List<PlayerReference> groupPlayers = new List<PlayerReference>();
            if (isGroupsModInstalled())
            {
                groupPlayers.AddRange(Groups.API.GroupPlayers());
            }

            return groupPlayers;
        }
    }
}