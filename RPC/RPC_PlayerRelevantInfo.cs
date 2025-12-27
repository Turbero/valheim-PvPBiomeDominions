namespace PvPBiomeDominions.RPC
{
    public class RPC_PlayerRelevantInfo
    {
        public string playerName;
        public int level;
        public int guildId;
        public bool isPvP;

        public string GetLevelText()
        {
            return level > 0 ? level.ToString() : "???";
        }
        public ZPackage GetPackage()
        {
            ZPackage pkg = new ZPackage();
            pkg.Write(playerName);
            pkg.Write(level);
            pkg.Write(guildId);
            pkg.Write(isPvP);
            return pkg;
        }

        public static RPC_PlayerRelevantInfo FromPackage(ZPackage pkg)
        {
            RPC_PlayerRelevantInfo info = new RPC_PlayerRelevantInfo
            {
                playerName = pkg.ReadString(),
                level = pkg.ReadInt(),
                guildId = pkg.ReadInt(),
                isPvP = pkg.ReadBool(),
            };
            return info;
        }
    }
}