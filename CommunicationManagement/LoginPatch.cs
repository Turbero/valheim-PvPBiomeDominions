using HarmonyLib;
using JetBrains.Annotations;

namespace PvPBiomeDominions.CommunicationManagement
{
	internal static class LoginPatch
	{
		[HarmonyPatch(typeof(Terminal), "AddString", typeof(string))]
		public static class LoginMessageChatPatch
		{
			[UsedImplicitly]
			public static bool Prefix(Terminal __instance, string text)
			{
				if (__instance is not Chat) return true;

				if (ConfigurationFile.welcomeInChatMessage.Value == ConfigurationFile.Toggle.On)
					return true;
				
				return !HasTextArrival(text);
			}
		}

		[HarmonyPatch(typeof(Chat), "SendText")]
		public static class LoginMessagePopupPatch
		{
			[UsedImplicitly]
			public static bool Prefix(Chat __instance, Talker.Type type, string text)
			{
				if (ConfigurationFile.welcomeInChatMessage.Value == ConfigurationFile.Toggle.On)
					return true;
				
				if (type == Talker.Type.Shout)
					return !HasTextArrival(text);
				return true;
			}
		}
		
		private static bool HasTextArrival(string text)
		{
			string translatedMessage = Localization.instance.Localize("$text_player_arrived").ToUpper();
			return (text ?? "").ToUpper().Contains(translatedMessage);
		}
	}
}