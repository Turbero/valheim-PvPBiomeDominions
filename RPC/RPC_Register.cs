using System;
using HarmonyLib;
using PvPBiomeDominions.PositionManagement.UI;
using PvPBiomeDominions.PvPManagement;

namespace PvPBiomeDominions.RPC
{
    [HarmonyPatch(typeof(Game), "Start")]
    public class GameStartPatch {
        private static void Prefix() {
            ZRoutedRpc.instance.Register("RPC_TombStoneAlertPlayer", new Action<long, bool>(TombStoneAlertsPatch.RPC_TombStoneAlertPlayer));
            ZRoutedRpc.instance.Register("RPC_RequestPlayerRelevantInfo", RPC_PlayersListPanel.RPC_RequestPlayerRelevantInfo);
            ZRoutedRpc.instance.Register("RPC_ResponsePlayerRelevantInfo", new Action<long, ZPackage>(RPC_PlayersListPanel.RPC_ResponsePlayerRelevantInfo));
            ZRoutedRpc.instance.Register("RPC_AddKillToKiller", new Action<long, string>(PvPKillCountPatch.RPC_AddKillToKiller));
        }
    }
}