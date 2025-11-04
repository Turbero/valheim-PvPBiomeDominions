using System;
using HarmonyLib;
using PvPBiomeDominions.Helpers;
using PvPBiomeDominions.PositionManagement.UI;
using PvPBiomeDominions.PvPManagement;

namespace PvPBiomeDominions.RPC
{
    [HarmonyPatch(typeof(Game), "Start")]
    public class GameStartPatch {
        private static void Prefix() {
            ZRoutedRpc.instance.Register("RPC_TombStoneAlertPlayer", new Action<long, bool>(TombStoneAlertsPatch.RPC_TombStoneAlertPlayer));
            ZRoutedRpc.instance.Register("RPC_RequestEpicMMOInfo", RPC_PlayersListPanel.RPC_RequestEpicMMOInfo);
            ZRoutedRpc.instance.Register("RPC_ResponseEpicMMOInfo", new Action<long, ZPackage>(RPC_PlayersListPanel.RPC_ResponseEpicMMOInfo));
        }
    }
}