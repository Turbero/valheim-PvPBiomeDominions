using System.Linq;
using HarmonyLib;

namespace PvPBiomeDominions.PvPManagement
{
    public class TombStoneAlertsPatch
    {
        [HarmonyPatch(typeof(Container), "Interact")]
        public class Container_Interact_Patch
        {
            static bool Prefix(Container __instance, Humanoid character)
            {
                if (character is Player thief)
                {
                    bool canLoot = thief.IsPVPEnabled()
                        ? ConfigurationFile.pvpAllowLootOtherTombstones.Value == ConfigurationFile.Toggle.On
                        : ConfigurationFile.pveAllowLootOtherTombstones.Value == ConfigurationFile.Toggle.On;
                    if (!canLoot)
                        MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, ConfigurationFile.forbidLootOtherTombstonesMessage.Value);
                    return canLoot;
                }
                return true; 
            }
            static void Postfix(Container __instance, Humanoid character, bool hold, bool alt, ref bool __result)
            {
                if (!ConfigurationFile.showMessageWhenLootingYourTombstone.Value || ConfigurationFile.pvpTombstoneLootAlertMessage.Value == string.Empty) return;

                TombStone tomb = __instance.GetComponentInParent<TombStone>();
                if (tomb == null) return;

                Logger.Log("Checking tombstone interact alert...");
                if (__instance.GetInventory().GetAllItems().Count > 0)
                {
                    processRPCWarning(tomb.GetOwnerName(), true);
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
                processRPCWarning(tomb.GetOwnerName(), false);
            }
        }

        private static void processRPCWarning(string owner, bool isInteractAlert)
        {
            Player localPlayer = Player.m_localPlayer;
            Logger.Log($"owner {owner} vs localPlayer {localPlayer.GetPlayerName()}");
            if (owner != localPlayer.GetPlayerName())
            {
                ZNet.PlayerInfo playerOwner = ZNet.instance.GetPlayerList().FirstOrDefault(p => p.m_name.Equals(owner));
                
                long ownerID = playerOwner.m_characterID.UserID;
                if (ownerID == 0)
                {
                    //owner not found (probably not online at the moment)
                    Logger.Log("Not found (probably disconnected)");
                    return;
                }

                Logger.Log($"Sending alert message to tombStone owner {owner} with ID {ownerID}...");
                ZRoutedRpc.instance.InvokeRoutedRPC(ownerID, "RPC_TombStoneAlertPlayer", isInteractAlert);
            }
            else
                Logger.Log("Recovering tombStone content...");
        }

        public static void RPC_TombStoneAlertPlayer(long sender, bool isInteractAlert)
        {
            Logger.Log("[RPC_TombStoneAlertPlayer] entered");
            
            //sender is the thief uid!
            ZNet.PlayerInfo playerThief = ZNet.instance.GetPlayerList().FirstOrDefault(p => p.m_characterID.UserID == sender);
            Player localPlayer = Player.m_localPlayer;
            Logger.Log("[RPC_TombStoneAlertPlayer] RPC sent to " + Player.m_localPlayer.GetPlayerName() + " from " + playerThief.m_name);
            
            if (isInteractAlert)
                localPlayer.Message(MessageHud.MessageType.Center, ConfigurationFile.pvpTombstoneLootAlertMessage.Value.Replace("{0}", playerThief.m_name));
            else
                localPlayer.Message(MessageHud.MessageType.Center, ConfigurationFile.pvpTombstoneDestroyAlertMessage.Value.Replace("{0}", playerThief.m_name));
        }
    }
}