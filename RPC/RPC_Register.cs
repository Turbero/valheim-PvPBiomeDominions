using System;
using HarmonyLib;
using PvPBiomeDominions.PvPManagement;

namespace PvPBiomeDominions.RPC
{
    [HarmonyPatch(typeof(Game), "Start")]
    public class GameStartPatch {
        private static void Prefix() {
            ZRoutedRpc.instance.Register("RPC_TombStoneAlertPlayer", new Action<long, bool>(TombStoneAlertsPatch.RPC_TombStoneAlertPlayer));
        }
    }
}