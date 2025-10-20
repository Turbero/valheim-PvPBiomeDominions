using System.IO;
using BepInEx;
using UnityEngine;
using UnityEngine.UI;

namespace PvPBiomeDominions.Helpers
{
    //[HarmonyPatch(typeof(Minimap), "Awake")]
    public class MinimapAwake_CaptureOrReplaceIcon
    {
        private static string iconFolder => Path.Combine(Paths.ConfigPath, "MinimapIcons");
        private static string vanillaIconPath => Path.Combine(iconFolder, "VanillaPlayerIcon.png");
        private static string customIconPath => Path.Combine(iconFolder, "CustomPlayerIcon.png");

        static void Postfix(Minimap __instance)
        {
            Logger.LogInfo("Downloading sprites...");
            // Create folder if it doesn't exist
            if (!Directory.Exists(iconFolder))
                Directory.CreateDirectory(iconFolder);

            // Obtener Image del minimapa grande y pequeño
            string[] paths = {
                "large/PublicPanel/PublicPosition/Background/Checkmark",
                "small/PublicPosition/Background/Checkmark"
            };

            foreach (var path in paths)
            {
                var img = __instance.transform.Find(path)?.GetComponent<Image>();
                if (img == null) continue;

                // if it doesn't exist the vanilla icon yet, capture it
                if (!File.Exists(vanillaIconPath) && img.sprite != null)
                {
                    Texture2D tex = img.sprite.texture;

                    // Create readable copy
                    Texture2D readableTex = new Texture2D(tex.width, tex.height, TextureFormat.RGBA32, false);
                    RenderTexture rt = RenderTexture.GetTemporary(tex.width, tex.height, 0);
                    Graphics.Blit(tex, rt);
                    RenderTexture prev = RenderTexture.active;
                    RenderTexture.active = rt;
                    readableTex.ReadPixels(new Rect(0,0,rt.width,rt.height), 0,0);
                    readableTex.Apply();
                    RenderTexture.active = prev;
                    RenderTexture.ReleaseTemporary(rt);

                    // Save PNG
                    byte[] pngData = readableTex.EncodeToPNG();
                    File.WriteAllBytes(vanillaIconPath, pngData);
                    Logger.LogInfo("Vanilla icon saved in: " + vanillaIconPath);
                }

                // If a custom icon exists, replace
                if (File.Exists(customIconPath))
                {
                    Sprite customSprite = LoadSpriteFromFile(customIconPath);
                    if (customSprite != null)
                    {
                        img.sprite = customSprite;
                        Logger.LogInfo("Custom icon applied: " + customIconPath);
                    }
                }
            }
        }
        
        // Create sprite from PNG
        private static Sprite LoadSpriteFromFile(string path)
        {
            if (!File.Exists(path)) return null;

            byte[] data = File.ReadAllBytes(path);
            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(data);
            return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        }
    }
}