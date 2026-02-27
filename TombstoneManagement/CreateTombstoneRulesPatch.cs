using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using PvPBiomeDominions.Helpers;

namespace PvPBiomeDominions.TombstoneManagement
{
    [HarmonyPatch(typeof(Player), "CreateTombStone")]
	internal static class TombstoneRulesPatch
	{
		[HarmonyPriority(Priority.VeryHigh)]
		private static void Prefix(Player __instance, out Dictionary<ItemDrop.ItemData, bool> __state)
		{
			List<ItemDrop.ItemData> list = new List<ItemDrop.ItemData>();
			__state = new Dictionary<ItemDrop.ItemData, bool>();

			Inventory inventory = GameManager.GetPrivateValue(__instance, "m_inventory") as Inventory;
			List<ItemDrop.ItemData> inventoryList = GameManager.GetPrivateValue(inventory, "m_inventory") as List<ItemDrop.ItemData>; 
			foreach (ItemDrop.ItemData item in inventoryList!)
			{
				string prefabName = Utils.GetPrefabName(item.m_dropPrefab);
				bool isHotbar = item.m_gridPos.y == 0;
				string invSection = isHotbar ? "hotbar" : "inventory";
				bool itemLossFlag = getFlagByPvPStatus("noItemLoss", __instance);
				bool keepEquippedFlag = item.m_equipped && getFlagByPvPStatus("keepEquipped", __instance);
				bool hotbarFlag = isHotbar && getFlagByPvPStatus("keepHotbar", __instance);
				if (itemLossFlag || keepEquippedFlag || hotbarFlag)
				{
					__state.Add(item, item.m_equipped);
					string reason = getReason(itemLossFlag, keepEquippedFlag);
					Logger.Log("Keeping " + prefabName + " in " + invSection + " due to the " + reason + ".");
					continue;
				}
				list.Add(item);
				Logger.Log("Dropping " + prefabName + " from " + invSection + ".");
			}
			GameManager.SetPrivateValue(inventory, "m_inventory", list);
		}

		private static bool getFlagByPvPStatus(string configName, Player player)
		{
			if (player.IsPVPEnabled())
			{
				if (configName.Equals("noItemLoss"))
					return ConfigurationFile.pvpNoItemLoss.Value == ConfigurationFile.Toggle.On;
				if (configName.Equals("keepEquipped"))
					return ConfigurationFile.pvpKeepEquipped.Value == ConfigurationFile.Toggle.On;
				if (configName.Equals("keepHotbar"))
					return ConfigurationFile.pvpKeepHotbar.Value == ConfigurationFile.Toggle.On;
			} else
			{
				if (configName.Equals("noItemLoss"))
					return ConfigurationFile.pveNoItemLoss.Value == ConfigurationFile.Toggle.On;
				if (configName.Equals("keepEquipped"))
					return ConfigurationFile.pveKeepEquipped.Value == ConfigurationFile.Toggle.On;
				if (configName.Equals("keepHotbar"))
					return ConfigurationFile.pveKeepHotbar.Value == ConfigurationFile.Toggle.On;
			}
			return false;
		}

		private static string getReason(bool itemLossFlag, bool keepEquippedFlag)
		{
			return itemLossFlag ? "noItemLoss setting" 
				: keepEquippedFlag  ? "keepEquipped setting" : "keepHotbar setting";
		}

		[HarmonyPriority(Priority.VeryLow)]
		private static void Postfix(Player __instance, Dictionary<ItemDrop.ItemData, bool> __state)
		{
			Inventory inventory = GameManager.GetPrivateValue(__instance, "m_inventory") as Inventory;
			List<ItemDrop.ItemData> inventoryList = GameManager.GetPrivateValue(inventory, "m_inventory") as List<ItemDrop.ItemData>;
			foreach (KeyValuePair<ItemDrop.ItemData, bool> item in __state)
			{
				var itemDropPrefabName = item.Key.m_dropPrefab.name;
				Logger.Log("Adding " + itemDropPrefabName + " back to inventory.");
				inventoryList?.Add(item.Key);
				Logger.Log($"Item {itemDropPrefabName} was equipped: {item.Value}");
				if (item.Value && (!itemDropPrefabName.StartsWith("BBH") || !itemDropPrefabName.EndsWith("Quiver")))
				{
					Logger.Log($"REequipping item {itemDropPrefabName}");
					__instance.UnequipItem(item.Key, false);
					__instance.EquipItem(item.Key, false);
				}
			}
			typeof(Inventory).GetMethod("Changed", BindingFlags.Instance | BindingFlags.NonPublic)?.Invoke(inventory, null);
		}
	}
}