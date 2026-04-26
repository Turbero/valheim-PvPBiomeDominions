using HarmonyLib;
using JetBrains.Annotations;

namespace PvPBiomeDominions.CommunicationManagement
{
	internal static class LogoutMessagePatch
	{
		[HarmonyPatch(typeof(ZNet), nameof(ZNet.Disconnect))]
		public static class RemoveDisconnectedPeerFromVerified
		{
			[UsedImplicitly]
			private static void Prefix(ZNetPeer peer, ref ZNet __instance)
			{
				if (!__instance.IsServer()) return;
				ZRoutedRpc.instance.InvokeRoutedRPC(ZRoutedRpc.Everybody, "RPC_LogoutMessage", peer.m_playerName);
			}
		}
	}
}