using System.Linq;
using PvPBiomeDominions.Helpers;
using PvPBiomeDominions.RPC;

namespace PvPBiomeDominions.PositionManagement.UI
{
    public class RPC_PlayersListPanel
    {
        public static void RPC_RequestPlayerRelevantInfo(long sender)
        {
            Logger.Log("[RPC_RequestPlayerRelevantInfo] entered");
            
            if (sender == ZDOMan.GetSessionID())
            {
                Logger.Log("[RPC_RequestPlayerRelevantInfo] skip selfmessage sent with ZroutedRpc.Everyone");
                return; //skip selfmessage sent with ZroutedRpc.Everyone
            }
            
            Player localPlayer = Player.m_localPlayer;
            if (localPlayer == null)
            {
                Logger.Log("localPlayer null. RPC message aborted.");
                return;
            }
            
            ZNet.PlayerInfo playerSender = ZNet.instance.GetPlayerList().FirstOrDefault(
                p => p.m_name != null &&
                p.m_characterID.UserID == sender);
            if (playerSender.m_name == null)
            {
                Logger.Log("PlayerSender not found. RPC message aborted.");
                return;
            }
            
            Logger.Log("[RPC_RequestPlayerRelevantInfo] RPC sent to " + Player.m_localPlayer.GetPlayerName() + " from " + playerSender.m_name);
            RPC_PlayerRelevantInfo playerRelevantInfo = new RPC_PlayerRelevantInfo
            {
                playerName = localPlayer.GetPlayerName(),
                level = EpicMMOSystem_API.GetLevel(),
                isPvP = localPlayer.IsPVPEnabled()
            };
            ZPackage pkg = playerRelevantInfo.GetPackage();
            
            ZRoutedRpc.instance.InvokeRoutedRPC(sender, "RPC_ResponsePlayerRelevantInfo", pkg);
        }
        
        public static void RPC_ResponsePlayerRelevantInfo(long sender, ZPackage pkg)
        {
            Logger.Log("[RPC_ResponsePlayerRelevantInfo] entered");
            
            ZNet.PlayerInfo playerSender = ZNet.instance.GetPlayerList().FirstOrDefault(p => p.m_characterID.UserID == sender);
            Player localPlayer = Player.m_localPlayer;
            Logger.Log("[RPC_ResponsePlayerRelevantInfo] RPC sent to " + localPlayer.GetPlayerName() + " from " + playerSender.m_name);

            RPC_PlayerRelevantInfo playerRelevantInfo = RPC_PlayerRelevantInfo.FromPackage(pkg);
            Logger.Log($"[RPC_ResponsePlayerRelevantInfo] playerRelevantInfo received: playerName {playerRelevantInfo.playerName} level: {playerRelevantInfo.level} and isPvP{playerRelevantInfo.isPvP}");
            
            MinimapUpdatePatch.panel.UpdatePlayerRelevantInfo(playerRelevantInfo);
        }
    }
}