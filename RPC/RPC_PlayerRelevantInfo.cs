namespace PvPBiomeDominions.RPC
{
    public class RPC_PlayerRelevantInfo
    {
        public string playerName;
        public int level;
        public bool isPvP;
        public int guildId;

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
            pkg.Write(guildId);
            return pkg;
        }

        public static RPC_PlayerRelevantInfo FromPackage(ZPackage pkg)
        {
            RPC_PlayerRelevantInfo info = new RPC_PlayerRelevantInfo
            {
                playerName = pkg.ReadString(),
                level = pkg.ReadInt(),
                isPvP = pkg.ReadBool(),
                guildId = pkg.ReadInt(),
            };
            return info;
        }
    }
}