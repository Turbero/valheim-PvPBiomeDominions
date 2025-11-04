using System.Linq;
using PvPBiomeDominions.Helpers;

namespace PvPBiomeDominions.PositionManagement.UI
{
    public class RPC_PlayersListPanel
    {
        public static void RPC_RequestEpicMMOInfo(long sender)
        {
            Player localPlayer = Player.m_localPlayer;
            if (sender == ZDOMan.GetSessionID()) return; //skip selfmessage sent with ZroutedRpc.Everyone
            
            Logger.Log("[RPC_RequestEpicMMOInfo] entered");
            ZNet.PlayerInfo playerSender = ZNet.instance.GetPlayerList().FirstOrDefault(p => p.m_characterID.UserID == sender);
            Logger.Log("[RPC_RequestEpicMMOInfo] RPC sent to " + Player.m_localPlayer.GetPlayerName() + " from " + playerSender.m_name);
            
            EpicMMOSystem_Info epicInfo = new EpicMMOSystem_Info
            {
                playerName = localPlayer.GetPlayerName(),
                level = EpicMMOSystem_API.GetLevel(),
                isPvP = localPlayer.IsPVPEnabled()
            };
            ZPackage pkg = epicInfo.GetPackage();
            
            ZRoutedRpc.instance.InvokeRoutedRPC(sender, "RPC_ResponseEpicMMOInfo", pkg);
        }
        
        public static void RPC_ResponseEpicMMOInfo(long sender, ZPackage pkg)
        {
            Logger.Log("[RPC_ResponseEpicMMOInfo] entered");
            
            ZNet.PlayerInfo playerSender = ZNet.instance.GetPlayerList().FirstOrDefault(p => p.m_characterID.UserID == sender);
            Player localPlayer = Player.m_localPlayer;
            Logger.Log("[RPC_ResponseEpicMMOInfo] RPC sent to " + localPlayer.GetPlayerName() + " from " + playerSender.m_name);

            EpicMMOSystem_Info epicInfo = EpicMMOSystem_Info.FromPackage(pkg);
            Logger.Log("[RPC_ResponseEpicMMOInfo] epicInfo.playerName: "+epicInfo.playerName);
            Logger.Log("[RPC_ResponseEpicMMOInfo] epicInfo.level: "+epicInfo.level);
            Logger.Log("[RPC_ResponseEpicMMOInfo] epicInfo.isPvP: "+epicInfo.isPvP);
            
            MinimapUpdatePatch.panel.UpdateEpicInfo(epicInfo);
        }
    }
}