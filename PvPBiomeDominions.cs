using BepInEx;
using HarmonyLib;
using UnityEngine;

namespace PvPBiomeDominions
{
    [BepInPlugin(GUID, NAME, VERSION)]
    [BepInDependency("org.bepinex.plugins.groups", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("Azumatt.WardIsLove", BepInDependency.DependencyFlags.SoftDependency)]
    public class PvPBiomeDominions : BaseUnityPlugin
    {
        public const string GUID = "Turbero.PvPBiomeDominions";
        public const string NAME = "PvP Biome Dominions";
        public const string VERSION = "1.3.2";

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

    [HarmonyPatch]
    public class NoWarnings
    {
	    [HarmonyPatch(typeof(Debug), nameof(Debug.LogWarning), typeof(object), typeof(Object))]
	    [HarmonyPrefix]
	    private static bool LogWarning(object message, Object context)
	    {
		    if (message.ToString().Contains("The LiberationSans SDF Font Asset was not found") || 
		        message.ToString().Contains("The character used for Ellipsis is not available in font asset"))
		    {
			    return false;
		    }

		    return true;
	    }
    }
}
