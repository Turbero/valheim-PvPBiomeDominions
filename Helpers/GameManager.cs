using System.Reflection;

namespace PvPBiomeDominions.Helpers
{
    public class GameManager
    {
        public static object GetPrivateValue(object obj, string name, BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.NonPublic)
        {
            return obj.GetType().GetField(name, bindingAttr)?.GetValue(obj);
        }

        public static bool isWackyEpicMMOSystemInstalled()
        {
            return BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("WackyMole.EpicMMOSystem");
        }
    }
}