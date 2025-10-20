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
        
        private static readonly string minimapLargeCheckMarkPath = "large/PublicPanel/PublicPosition/Background/Checkmark";
        
        public static void UpdateMapSelectorIcon()
        {
            if (spriteIconVanillaImage == null)
            {
                Logger.Log("Initializing...");
                InitSprites();
            }
            
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

        private static void InitSprites()
        {
            if (spriteIconVanillaImage != null) return;
            var sprite = Minimap.instance.transform.Find(minimapLargeCheckMarkPath)?.GetComponent<Image>().sprite;
            if (sprite == null) return;
            spriteIconVanillaImage = sprite;
            Logger.Log("Vanilla sprite stored.");
            
            spriteBlueIconImage = LoadSpriteFromEmbedded("icons.minimap-valheim-icon-blue.png");
            Logger.Log("Blue sprite loaded.");
        }
        
        private static Sprite LoadSpriteFromEmbedded(string embeddedPath)
        {
            try
            {
                // Leer los datos de la imagen
                byte[] imageData = ReadEmbeddedFileBytes(embeddedPath);
                if (imageData == null)
                    Logger.Log("imageData is null");

                // Crear textura
                Texture2D texture = new Texture2D(2, 2); // El tamaño inicial no importa; será ajustado.
                if (!texture.LoadImage(imageData))
                {
                    Debug.LogError("Failed to load texture from image data.");
                    return null;
                }

                // Crear sprite a partir de la textura
                return Sprite.Create(
                    texture,
                    new Rect(0, 0, texture.width, texture.height),
                    new Vector2(0.5f, 0.5f) // Punto de pivote
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