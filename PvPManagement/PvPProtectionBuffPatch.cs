using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using PvPBiomeDominions.Helpers;
using UnityEngine;

namespace PvPBiomeDominions.PvPManagement
{
    public static class PvPSpawnProtectionTracker
    {
        private static readonly HashSet<long> _killedByPlayer = new();

        private const string ZDO_KEY = "pvp_protection_until";

        public static void MarkKilledByPlayer(Player player)
        {
            _killedByPlayer.Add(player.GetPlayerID());
        }

        public static bool WasKilledByPlayer(Player player)
        {
            return _killedByPlayer.Remove(player.GetPlayerID());
        }

        public static void SetProtection(Player player, float seconds)
        {
            ZNetView m_nview = (ZNetView)GameManager.GetPrivateValue(player, "m_nview");
            if (m_nview == null || m_nview.GetZDO() == null)
                return;

            long protectionUntil = ZNet.instance.GetTime().Ticks +
                                   (long)(seconds * 10000000);

            m_nview.GetZDO().Set(ZDO_KEY, protectionUntil);
        }

        public static bool HasProtection(Player player)
        {
            ZNetView m_nview = (ZNetView)GameManager.GetPrivateValue(player, "m_nview");
            if (m_nview == null || m_nview.GetZDO() == null)
                return false;
            
            if (m_nview?.GetZDO() == null)
                return false;

            long until = m_nview.GetZDO().GetLong(ZDO_KEY, 0L);

            if (until == 0)
                return false;

            return ZNet.instance.GetTime().Ticks < until;
        }

        public static void ClearProtection(Player player)
        {
            ZNetView m_nview = (ZNetView)GameManager.GetPrivateValue(player, "m_nview");
            if (m_nview == null || m_nview.GetZDO() == null)
                return;
            
            if (m_nview?.GetZDO() == null)
                return;

            m_nview.GetZDO().Set(ZDO_KEY, 0L);
        }
    }

    // -------------------------------------------------------
    // Detect PvP death
    // -------------------------------------------------------

    [HarmonyPatch(typeof(Player), "OnDeath")]
    public static class Player_OnDeath_Patch
    {
        public static void Prefix(Player __instance)
        {
            HitData hit = (HitData)typeof(Player)
                .GetField("m_lastHit", BindingFlags.Instance | BindingFlags.NonPublic)!
                .GetValue(__instance);

            if (hit == null)
                return;

            Character attacker = hit.GetAttacker();

            if (attacker is Player attackerPlayer &&
                attackerPlayer != __instance)
            {
                PvPSpawnProtectionTracker.MarkKilledByPlayer(__instance);
            }
        }
    }

    // -------------------------------------------------------
    // Apply protection on respawn
    // -------------------------------------------------------

    [HarmonyPatch(typeof(Player), nameof(Player.OnSpawned))]
    public class SE_PvPSpawnProtectionPatch
    {
        public static void Postfix(Player __instance)
        {
            if (ConfigurationFile.waitingTimeAfterDyingToFightPlayersAgain.Value <= 0)
                return;

            if (!PvPSpawnProtectionTracker.WasKilledByPlayer(__instance))
                return;

            float seconds = ConfigurationFile
                .waitingTimeAfterDyingToFightPlayersAgain.Value * 60f;

            Logger.Log("Adding PvP ZDO protection to " + __instance.GetPlayerName());

            // 🔹 Sync protection through ZDO
            PvPSpawnProtectionTracker.SetProtection(__instance, seconds);

            // 🔹 Add visual StatusEffect (optional, purely cosmetic)
            var se = ScriptableObject.CreateInstance<SE_PvPSpawnImmunity>();
            se.m_ttl = seconds;
            __instance.GetSEMan().AddStatusEffect(se);
        }
    }

    // -------------------------------------------------------
    // Block PvP damage
    // -------------------------------------------------------

    [HarmonyPatch(typeof(Character), nameof(Character.ApplyDamage))]
    public class CheckPvPProtectionBuffPatch
    {
        [HarmonyPrefix]
        public static bool Prefix(Character __instance, HitData hit)
        {
            if (__instance is not Player victim)
                return true;

            Character attacker = hit.GetAttacker();

            if (attacker is not Player attackerPlayer)
                return true;

            // Victim protected → cannot receive PvP damage
            if (PvPSpawnProtectionTracker.HasProtection(victim))
            {
                Logger.Log("Blocked PvP damage: victim protected");
                return false;
            }

            // Attacker protected → cannot deal PvP damage
            if (PvPSpawnProtectionTracker.HasProtection(attackerPlayer))
            {
                Logger.Log("Blocked PvP damage: attacker protected");
                return false;
            }

            return true;
        }
    }
}