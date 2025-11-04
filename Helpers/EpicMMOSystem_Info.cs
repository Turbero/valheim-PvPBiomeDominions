namespace PvPBiomeDominions.Helpers
{
    public class EpicMMOSystem_Info
    {
        public string playerName;
        public int level;
        public bool isPvP;

        public ZPackage GetPackage()
        {
            ZPackage pkg = new ZPackage();
            pkg.Write(playerName);
            pkg.Write(level);
            pkg.Write(isPvP);
            return pkg;
        }

        public static EpicMMOSystem_Info FromPackage(ZPackage pkg)
        {
            EpicMMOSystem_Info info = new EpicMMOSystem_Info
            {
                playerName = pkg.ReadString(),
                level = pkg.ReadInt(),
                isPvP = pkg.ReadBool(),
            };
            return info;
        }
    }
}