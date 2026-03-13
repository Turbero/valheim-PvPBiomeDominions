using System.Linq;
using HarmonyLib;

namespace PvPBiomeDominions.TombstoneManagement
{
    public class TombStoneAlertsPatch
    {
        [HarmonyPatch(typeof(Container), "Interact")]
        public class Container_Interact_Patch
        {
            static bool Prefix(Container __instance, Humanoid character, bool hold, bool alt)
            {
                if (ConfigurationFile.showMessageWhenLootingYourTombstone.Value == ConfigurationFile.Toggle.Off ||
                    ConfigurationFile.tombstoneLootAlertMessage.Value == string.Empty)
                    return true;

                TombStone tomb = __instance.GetComponentInParent<TombStone>();
                if (tomb != null && character is Player thief)
                {
                    string owner = tomb.GetOwnerName();
                    Logger.Log($"Container.Interact | Prefix | owner {owner} vs localPlayer {thief.GetPlayerName()}");
                    if (owner != thief.GetPlayerName())
                    {
                        bool canLoot = thief.IsPVPEnabled()
                            ? ConfigurationFile.pvpAllowLootOtherTombstones.Value == ConfigurationFile.Toggle.On
                            : ConfigurationFile.pveAllowLootOtherTombstones.Value == ConfigurationFile.Toggle.On;
                        if (!canLoot)
                        {
                            Logger.Log("cannot loot that!");
                            MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, ConfigurationFile.forbidLootOtherTombstonesMessage.Value);
                            return false;
                        }
                    }
                }

                return true;
            }
            static void Postfix(Container __instance, Humanoid character, bool hold, bool alt, ref bool __result)
            {
                if (ConfigurationFile.showMessageWhenLootingYourTombstone.Value == ConfigurationFile.Toggle.Off ||
                    ConfigurationFile.tombstoneLootAlertMessage.Value == string.Empty ||
                    !__result) return;

                TombStone tomb = __instance.GetComponentInParent<TombStone>();
                if (tomb == null) return;
                
                string owner = tomb.GetOwnerName();
                Player localPlayer = Player.m_localPlayer;
                Logger.Log($"Container.Interact | Postfix | owner {owner} vs localPlayer {localPlayer.GetPlayerName()}");
                if (owner != localPlayer.GetPlayerName() && __instance.GetInventory().GetAllItems().Count > 0)
                {
                    Logger.Log("Checking tombstone interact alert...");
                    sendAlertToTombstoneOwner(tomb.GetOwnerName(), true);
                }
            }
        }

        [HarmonyPatch(typeof(Container), "TakeAll")]
        public class Container_TakeAll_Patch
        {
            static void Postfix(Container __instance, Humanoid character, ref bool __result)
            {
                if (ConfigurationFile.showMessageWhenLootingYourTombstone.Value == ConfigurationFile.Toggle.Off ||
                    ConfigurationFile.tombstoneDestroyAlertMessage.Value == string.Empty ||
                    !__result) return;

                TombStone tomb = __instance.GetComponentInParent<TombStone>();
                if (tomb == null) return;

                Logger.Log("Container.TakeAll | Checking tombstone alert...");
                string owner = tomb.GetOwnerName();
                Player localPlayer = Player.m_localPlayer;
                Logger.Log($"Container.TakeAll | owner {owner} vs localPlayer {localPlayer.GetPlayerName()}");
                if (owner != localPlayer.GetPlayerName())
                {
                    sendAlertToTombstoneOwner(owner, false);
                }
            }
        }

        private static void sendAlertToTombstoneOwner(string owner, bool isInteractAlert)
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

        public static void RPC_TombStoneAlertPlayer(long sender, bool isInteractAlert)
        {
            Logger.Log("[RPC_TombStoneAlertPlayer] entered");
            
            //sender is the thief uid!
            ZNet.PlayerInfo playerThief = ZNet.instance.GetPlayerList().FirstOrDefault(p => p.m_characterID.UserID == sender);
            Player localPlayer = Player.m_localPlayer;
            Logger.Log("[RPC_TombStoneAlertPlayer] RPC sent to " + Player.m_localPlayer.GetPlayerName() + " from " + playerThief.m_name);
            
            if (isInteractAlert)
                localPlayer.Message(MessageHud.MessageType.Center, ConfigurationFile.tombstoneLootAlertMessage.Value.Replace("{0}", playerThief.m_name));
            else
                localPlayer.Message(MessageHud.MessageType.Center, ConfigurationFile.tombstoneDestroyAlertMessage.Value.Replace("{0}", playerThief.m_name));
        }
    }
}