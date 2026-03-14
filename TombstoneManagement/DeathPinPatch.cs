using System.Collections.Generic;
using HarmonyLib;
using PvPBiomeDominions.Helpers;
using UnityEngine;

namespace PvPBiomeDominions.TombstoneManagement
{
	public enum DeathPinMapRule
	{
		Default,
		RemoveWhenLootingOrEmpty,
		NoPins
	}
	
	class PinManagement
	{
		public static void RemovePlayerPin(Component __instance, bool inventoryWasEmpty = false)
		{
			if (inventoryWasEmpty) return;
			
			//Find suitable pin
			Minimap.PinData playerPin = FindPlayerPin(__instance.transform.position, 10f);
			if (playerPin is not { m_type: Minimap.PinType.Death })
				playerPin = FindPlayerPin(__instance.transform.position, 5f);
			
			if (playerPin != null)
			{
				Logger.Log($"Removing pin '{playerPin.m_name}'");

				//Same updates as in Minimap
				bool flag = (Chat.instance == null || !Chat.instance.HasFocus()) &&
				            !Console.IsVisible() &&
				            !TextInput.IsVisible() &&
				            !Menu.IsVisible() &&
				            !InventoryGui.IsVisible();
				float deltaTime = Time.deltaTime;
				Minimap.instance.SetMapMode(Minimap.MapMode.Large);
				Minimap.instance.RemovePin(playerPin);
				GameManager.RunPrivateMethod(Minimap.instance, "UpdateMap", new object[] { Player.m_localPlayer, deltaTime, flag });
				GameManager.RunPrivateMethod(Minimap.instance, "UpdateDynamicPins", new object[] { deltaTime });
				GameManager.RunPrivateMethod(Minimap.instance, "UpdatePins");
				GameManager.RunPrivateMethod(Minimap.instance, "UpdateBiome", new object[] { Player.m_localPlayer });
				GameManager.RunPrivateMethod(Minimap.instance, "UpdateNameInput");
				GameManager.RunPrivateMethod(Minimap.instance, "UpdatePlayerPins", new object[] { deltaTime });
				Minimap.instance.SetMapMode(0);
			}
		}

		private static Minimap.PinData FindPlayerPin(Vector3 pos, float radius)
		{
			Minimap.PinData closestPin = null;
			float maxDistance = 999999f;
			List<Minimap.PinData> m_pins = (List<Minimap.PinData>)GameManager.GetPrivateValue(Minimap.instance, "m_pins");
			foreach (Minimap.PinData pin in m_pins)
			{
				if (pin.m_save && pin.m_uiElement != null)
				{
					float distance = Utils.DistanceXZ(pos, pin.m_pos);
					if (distance < radius && (distance < maxDistance || closestPin == null))
					{
						closestPin = pin;
						maxDistance = distance;
					}
				}
			}
			return closestPin;
		}
	}

	//1) First, death comes (even in Valhalla)
	[HarmonyPatch(typeof(Player), "OnDeath")]
	public class PlayerOnDeathPatch
	{
		private static void Postfix(Player __instance)
		{
			if (PlayerCreateTombStonePatch.playerInventoryWasEmpty && ConfigurationFile.deathPinMapRule.Value == DeathPinMapRule.RemoveWhenLootingOrEmpty)
			{
				Logger.Log("No pin when player inventory is empty.");
				PinManagement.RemovePlayerPin(__instance, PlayerCreateTombStonePatch.playerInventoryWasEmpty);
				PlayerCreateTombStonePatch.playerInventoryWasEmpty = false;
			}
		}
	}
	
	//2)) Second, calculate inventory when tombstone is created (or not)
	[HarmonyPatch(typeof(Player), "CreateTombStone")]
	public class PlayerCreateTombStonePatch
	{
		public static bool playerInventoryWasEmpty; //temporary value

		private static void Prefix(Player __instance)
		{
			if (__instance.GetInventory() != null && __instance == Player.m_localPlayer)
				playerInventoryWasEmpty = __instance.GetInventory().NrOfItems() == 0;
		}
	}
	
	//3) Third, pin is to be added if NoPins is selected
	[HarmonyPatch(typeof(Minimap), "AddPin")]
	internal static class MinimapAddPinPatch
	{
		private static bool Prefix(Minimap __instance, Vector3 pos, Minimap.PinType type, string name, bool save, bool isChecked, long ownerID = 0L)
		{
			if (ConfigurationFile.deathPinMapRule.Value == DeathPinMapRule.NoPins)
				return type != Minimap.PinType.Death;
			return true;
		}
	}
	
	//4) And last, pin is removed if tombstone was looted
	[HarmonyPatch(typeof(TombStone), "GiveBoost")]
	public class TombstoneGiveBoostPatch
	{
		private static void Prefix(TombStone __instance)
		{
			if (ConfigurationFile.deathPinMapRule.Value == DeathPinMapRule.Default)
				return;
			
			PinManagement.RemovePlayerPin(__instance);
		}
	}
}