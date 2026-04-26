using PvPBiomeDominions.Helpers;

namespace PvPBiomeDominions.RPC
{
    public class RPC_Communication
    {
        public static void PlayerLogoutRPC(long sender, string playerName)
        {
            Chat chat = Chat.instance;
            if (chat != null)
            {
                chat.AddString("[Server]", ConfigurationFile.logoutMessage.Value.Replace("{0}", playerName), Talker.Type.Shout);
                GameManager.SetPrivateValue(chat, "m_hideTimer", 0.0f);
                chat.m_chatWindow.gameObject.SetActive(true);
                chat.m_input.gameObject.SetActive(true);
            }
        }
    }
}