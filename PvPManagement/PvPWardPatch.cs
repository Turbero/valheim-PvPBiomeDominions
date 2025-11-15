using HarmonyLib;

namespace PvPBiomeDominions.PvPManagement
{
    [HarmonyPatch(typeof(Player), "TryPlacePiece")]
    public static class PvPWardPatch
    {
        static bool Prefix(Player __instance, Piece piece)
        {
            string name = piece.name.Replace("(Clone)", "");
            if (name.ToLowerInvariant().Contains("guard_stone") || ConfigurationFile.wardModsPrefabIds.Value.ToLowerInvariant().Contains(name.ToLowerInvariant()))
            {
                Logger.Log("Ward creation detected. Checking...");
                if (!ConfigurationFile.IsWardCreationAllowedInCurrentBiomeRule())
                {
                    Logger.Log("Not allowed in current biome.");
                    __instance.Message(MessageHud.MessageType.Center, ConfigurationFile.wardCreationNotAllowed.Value);
                    return false;
                }
            }
            Logger.Log("Allowed!");
            return true;
        }
    }
}