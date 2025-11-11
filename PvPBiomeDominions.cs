using BepInEx;
using HarmonyLib;

namespace PvPBiomeDominions
{
    [BepInPlugin(GUID, NAME, VERSION)]
    [BepInDependency("org.bepinex.plugins.groups", BepInDependency.DependencyFlags.SoftDependency)]
    public class PvPBiomeDominions : BaseUnityPlugin
    {
        public const string GUID = "Turbero.PvPBiomeDominions";
        public const string NAME = "PvP Biome Dominions";
        public const string VERSION = "1.2.2";

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
