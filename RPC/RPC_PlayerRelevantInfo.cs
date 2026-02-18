namespace PvPBiomeDominions.RPC
{
    public class RPC_PlayerRelevantInfo
    {
        public string playerName;
        public int level;
        public bool isPvP;
        public string guildName;
        public int guildIconId;

        public string GetLevelText()
        {
            return level > 0 ? level.ToString() : "???";
        }
        public ZPackage GetPackage()
        {
            ZPackage pkg = new ZPackage();
            pkg.Write(playerName);
            pkg.Write(level);
            pkg.Write(isPvP);
            pkg.Write(guildName);
            pkg.Write(guildIconId);
            return pkg;
        }

        public static RPC_PlayerRelevantInfo FromPackage(ZPackage pkg)
        {
            RPC_PlayerRelevantInfo info = new RPC_PlayerRelevantInfo
            {
                playerName = pkg.ReadString(),
                level = pkg.ReadInt(),
                isPvP = pkg.ReadBool(),
                guildName = pkg.ReadString(),
                guildIconId = pkg.ReadInt()
            };
            return info;
        }
    }
}