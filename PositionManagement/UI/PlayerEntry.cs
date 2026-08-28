using TMPro;
using UnityEngine.UI;

namespace PvPBiomeDominions.PositionManagement.UI
{
    public class PlayerEntry
    {
        public string name;
        public int level;
        public bool isPvP;
        public string guildName;
        public int guildIconId;
        
        public Image iconPlayer;
        public TextMeshProUGUI killsTimesUI;
        public TextMeshProUGUI killedByTimesUI;
        public TextMeshProUGUI levelUI;
        public Image guildIconUI;

        public string GetLevelText()
        {
            return level > 0 ? level.ToString() : "???";
        }

        public void rebind(TextMeshProUGUI killsValue, TextMeshProUGUI killedByValue, Image playerIcon, Image imageGuild)
        {
            killsTimesUI = killsValue;
            killedByTimesUI = killedByValue;
            iconPlayer = playerIcon;
            guildIconUI = imageGuild;
        }
    }
}