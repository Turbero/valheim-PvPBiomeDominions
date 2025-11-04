using HarmonyLib;
using PvPBiomeDominions.Helpers;

namespace PvPBiomeDominions.PositionManagement
{
    //[HarmonyPatch(typeof(MessageHud), nameof(MessageHud.ShowMessage))]
    public static class MessageHud_ShowMessage_Patch
    {
        static bool Prefix(MessageHud __instance, MessageHud.MessageType type, string text)
        {
            Logger.Log("Text to look up: " + text + ". " + text.Contains("$msg_welcome"));
            if (type == MessageHud.MessageType.Center)
            {
                bool sharePosition = (bool)GameManager.GetPrivateValue(ZNet.instance, "m_publicReferencePosition");
                if (!sharePosition && text.Contains("$msg_welcome"))
                    return false;
            }

            return true;
        }
    }

}