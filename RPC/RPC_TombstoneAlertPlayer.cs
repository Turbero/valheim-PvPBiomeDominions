using System.Linq;

namespace PvPBiomeDominions.RPC
{
    public class RPC_TombstoneAlertPlayer
    {
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