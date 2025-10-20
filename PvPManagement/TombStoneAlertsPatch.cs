using HarmonyLib;

namespace PvPBiomeDominions.PvPManagement
{
    [HarmonyPatch(typeof(Container), "Interact")]
    public class Container_Interact_Patch
    {
        static void Postfix(Container __instance, Humanoid character, bool hold, bool alt, ref bool __result)
        {
            if (ConfigurationFile.pvpTombstoneLootAlertMessage.Value == string.Empty) return;
            
            TombStone tomb = __instance.GetComponentInParent<TombStone>();
            if (tomb == null) return;
            
            Logger.Log("Checking tombstone interact alert...");
            if (__instance.GetInventory().GetAllItems().Count > 0)
            {
                string owner = tomb.GetOwnerName();

                Player localPlayer = Player.m_localPlayer;
                Logger.Log($"owner {owner} vs localPlayer {localPlayer.GetPlayerName()}");
                if (owner != localPlayer.GetPlayerName())
                {
                    Logger.Log($"Sending interact alert message to {owner}...");
                    Player playerOwner = Player.GetAllPlayers().Find(p => p.GetPlayerName().Equals(owner));
                    if (playerOwner != null)
                        playerOwner.Message(MessageHud.MessageType.Center, ConfigurationFile.pvpTombstoneLootAlertMessage.Value.Replace("{0}", localPlayer.GetPlayerName()));
                }
            }
        }
    }

    [HarmonyPatch(typeof(Container), "TakeAll")]
    public class Container_TakeAll_Patch
    {
        static void Postfix(Container __instance, Humanoid character, ref bool __result)
        {
            if (ConfigurationFile.pvpTombstoneDestroyAlertMessage.Value == string.Empty || !__result) return;
            
            TombStone tomb = __instance.GetComponentInParent<TombStone>();
            if (tomb == null) return;

            Logger.Log("Checking tombstone takeAll alert...");
            
            string owner = tomb.GetOwnerName();
            Player localPlayer = Player.m_localPlayer;
            Logger.Log($"owner {owner} vs localPlayer {localPlayer.GetPlayerName()}");
            if (owner != localPlayer.GetPlayerName())
            {
                Logger.Log($"Sending takeAll alert message to {owner}...");
                Player playerOwner = Player.GetAllPlayers().Find(p => p.GetPlayerName().Equals(owner));
                if (playerOwner != null)
                    playerOwner.Message(MessageHud.MessageType.Center, ConfigurationFile.pvpTombstoneDestroyAlertMessage.Value.Replace("{0}", localPlayer.GetPlayerName()));
            }
        }
    }
}