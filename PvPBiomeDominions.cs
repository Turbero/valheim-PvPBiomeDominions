using BepInEx;
using HarmonyLib;

namespace PvPBiomeDominions
{
    [BepInPlugin(GUID, NAME, VERSION)]
    public class PvPBiomeDominions : BaseUnityPlugin
    {
        public const string GUID = "Turbero.PvPBiomeDominions";
        public const string NAME = "PvP Biome Dominions";
        public const string VERSION = "1.0.1";

        private readonly Harmony harmony = new Harmony(GUID);

        void Awake()
        {
            ConfigurationFile.LoadConfig(this);
            
            harmony.PatchAll();
        }

        void onDestroy()
        {
            harmony.UnpatchSelf();
        }
    }
}
