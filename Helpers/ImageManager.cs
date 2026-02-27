using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace PvPBiomeDominions.Helpers
{
    public class ImageManager
    {
        private static Sprite spriteIconVanillaImage;
        private static Sprite spriteBlueIconImage;
        private static Sprite spriteGroupIconImage;
        
        private static readonly string minimapLargeCheckMarkPath = "large/PublicPanel/PublicPosition/Background/Checkmark";

        public static Sprite getSpriteIconVanillaImage()
        {
            if (spriteIconVanillaImage == null)
                InitVanillaSprite();
                
            return spriteIconVanillaImage;
        }

        public static Sprite getSpriteIconPVEImage()
        {
            if (spriteBlueIconImage == null)
            {
                InitBlueIconSprite();
            }

            return spriteBlueIconImage;
        }

        public static Sprite getSpriteGroupIconImage()
        {
            if (spriteGroupIconImage == null)
                InitGroupIconSprite();

            return spriteGroupIconImage;
        }

        public static void UpdateMapSelectorIcon()
        {
            Player localPlayer = Player.m_localPlayer;
            if (localPlayer == null) return;

            if (localPlayer.IsPVPEnabled())
            {
                Minimap.instance.transform.Find(minimapLargeCheckMarkPath).GetComponent<Image>().sprite = spriteIconVanillaImage;
                Minimap.instance.transform.Find(minimapLargeCheckMarkPath).GetComponent<RectTransform>().sizeDelta = new Vector2(32, 32);
            }
            else
            {
                Minimap.instance.transform.Find(minimapLargeCheckMarkPath).GetComponent<Image>().sprite = spriteBlueIconImage;
                Minimap.instance.transform.Find(minimapLargeCheckMarkPath).GetComponent<RectTransform>().sizeDelta = new Vector2(38, 38);
            }
        }

        private static void InitGroupIconSprite()
        {
            spriteGroupIconImage = LoadSpriteFromEmbedded("icons.minimap-valheim-icon-base.png");
            Color[] groupPixels = loadTexture("icons.minimap-valheim-icon-base.png").GetPixels();
            for (int i = 0; i < groupPixels.Length; ++i)
            {
                if (groupPixels[i].r > 0.5 && groupPixels[i].b < 0.5 && groupPixels[i].g < 0.5)
                {
                    groupPixels[i] = new Color32(0, 255, 0, 255);
                }
            }
            spriteGroupIconImage.texture.SetPixels(groupPixels);
            spriteGroupIconImage.texture.Apply();
            Logger.LogInfo("Green(Group) sprite loaded.");
        }

        private static void InitBlueIconSprite()
        {
            spriteBlueIconImage = LoadSpriteFromEmbedded("icons.minimap-valheim-icon-base.png");
            Color[] bluePixels = loadTexture("icons.minimap-valheim-icon-base.png").GetPixels();
            for (int i = 0; i < bluePixels.Length; ++i)
            {
                if (bluePixels[i].r > 0.5 && bluePixels[i].b < 0.5 && bluePixels[i].g < 0.5)
                {
                    bluePixels[i] = new Color32(0, 188, 255, 196);
                }
            }
            spriteBlueIconImage.texture.SetPixels(bluePixels);
            spriteBlueIconImage.texture.Apply();
            Logger.LogInfo("Blue(PvE) sprite loaded.");
        }

        private static void InitVanillaSprite()
        {
            var sprite = Minimap.instance.transform.Find(minimapLargeCheckMarkPath)?.GetComponent<Image>()?.sprite;
            if (sprite != null)
            {
                spriteIconVanillaImage = sprite;
                Logger.LogInfo("Vanilla(PvP) sprite stored.");
            } else
                Logger.LogWarning("Vanilla sprite not found.");
        }
        
        private static Texture2D loadTexture(string name)
        {
            Texture2D texture = new(0, 0);
            texture.LoadImage(ReadEmbeddedFileBytes(name));
            return texture;
        }
        
        private static Sprite LoadSpriteFromEmbedded(string embeddedPath)
        {
            try
            {
                // Read image data
                byte[] imageData = ReadEmbeddedFileBytes(embeddedPath);
                if (imageData == null)
                    Logger.Log("imageData is null");

                // Create texture
                Texture2D texture = new Texture2D(2, 2); // doesn't matter init size. It will be adjusted
                if (!texture.LoadImage(imageData))
                {
                    Debug.LogError("Failed to load texture from image data.");
                    return null;
                }

                // Create sprite from texture
                return Sprite.Create(
                    texture,
                    new Rect(0, 0, texture.width, texture.height),
                    new Vector2(0.5f, 0.5f) // Pivot point
                );
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Failed to load image from file: {ex.Message}");
                return null;
            }
        }
        
        private static byte[] ReadEmbeddedFileBytes(string name)
        {
            using MemoryStream stream = new MemoryStream();
            Assembly.GetExecutingAssembly().GetManifestResourceStream("PvPBiomeDominions." + name)?.CopyTo(stream);
            return stream.ToArray();
        }
    }
}