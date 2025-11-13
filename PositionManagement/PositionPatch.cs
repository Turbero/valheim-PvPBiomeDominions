using HarmonyLib;
using PvPBiomeDominions.Helpers;
using PvPBiomeDominions.Helpers.WardIsLove;

namespace PvPBiomeDominions.PositionManagement
{
    [HarmonyPatch(typeof(ZNet), nameof(ZNet.SetPublicReferencePosition))]
    public class PublicReferencePositionPatch
    {
        public static void Postfix(ref bool pub, ref bool ___m_publicReferencePosition)
        {
            if (ConfigurationFile.positionAdminExempt.Value == ConfigurationFile.Toggle.On && ConfigurationFile.ConfigSync.IsAdmin)
                return;
            
            // Ward rule first
            bool isInsideWard = WardIsLovePlugin.IsInsideWard();
            var wardPositionRule = ConfigurationFile.positionRuleInWards.Value;
            if (isInsideWard && wardPositionRule != ConfigurationFile.PositionSharingWardRule.FollowBiomeRule)
            {
                if (wardPositionRule == ConfigurationFile.PositionSharingWardRule.PlayerChoice)
                    return;
                
                ___m_publicReferencePosition = wardPositionRule == ConfigurationFile.PositionSharingWardRule.ShowPlayer;
                return;
            }
            
            // Then biome rule
            ConfigurationFile.PositionSharingBiomeRule biomePositionBiomeRule = ConfigurationFile.getCurrentBiomeRulePosition();
            if (biomePositionBiomeRule == ConfigurationFile.PositionSharingBiomeRule.PlayerChoice)
                return;
            
            ___m_publicReferencePosition = biomePositionBiomeRule == ConfigurationFile.PositionSharingBiomeRule.ShowPlayer;
            
            if (___m_publicReferencePosition)
                ImageManager.UpdateMapSelectorIcon();
        }
    }

    [HarmonyPatch(typeof(Player), nameof(Player.OnSpawned))]
    public class PlayerOnSpawnedPatch
    {
        public static void Postfix(Player __instance)
        {
            if (ConfigurationFile.positionAdminExempt.Value == ConfigurationFile.Toggle.On && ConfigurationFile.ConfigSync.IsAdmin)
                return;

            // First ward rule
            bool isInsideWard = WardIsLovePlugin.IsInsideWard();
            var wardPositionRule = ConfigurationFile.positionRuleInWards.Value;
            if (isInsideWard && wardPositionRule != ConfigurationFile.PositionSharingWardRule.FollowBiomeRule)
            {
                if (wardPositionRule == ConfigurationFile.PositionSharingWardRule.PlayerChoice)
                    return;

                GameManager.SetPrivateValue(ZNet.instance, "m_publicReferencePosition",
                    wardPositionRule == ConfigurationFile.PositionSharingWardRule.ShowPlayer);
                return;
            }

            // Then biome rule
            ConfigurationFile.PositionSharingBiomeRule biomePositionBiomeRule = ConfigurationFile.getCurrentBiomeRulePosition();
            if (biomePositionBiomeRule == ConfigurationFile.PositionSharingBiomeRule.PlayerChoice)
                return;
            
            GameManager.SetPrivateValue(ZNet.instance, "m_publicReferencePosition",
                biomePositionBiomeRule == ConfigurationFile.PositionSharingBiomeRule.ShowPlayer);
            
            if (ZNet.instance.IsReferencePositionPublic())
                ImageManager.UpdateMapSelectorIcon();
        }
    }
}