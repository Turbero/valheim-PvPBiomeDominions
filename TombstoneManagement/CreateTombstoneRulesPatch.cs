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
				bool flag = ConfigurationFile.noItemLoss.Value == ConfigurationFile.Toggle.On;
				bool flag2 = item.m_equipped && ConfigurationFile.keepEquipped.Value == ConfigurationFile.Toggle.On;
				bool flag3 = isHotbar && ConfigurationFile.keepHotbar.Value == ConfigurationFile.Toggle.On;
				if (flag || flag2 || flag3)
				{
					__state.Add(item, item.m_equipped);
					string reason = flag 
						? "noItemLoss setting" 
						: flag2 
							? "keepEquipped setting" 
							: "keepHotbar setting";
					Logger.Log("Keeping " + prefabName + " in " + invSection + " due to the " + reason + ".");
					continue;
				}
				list.Add(item);
				Logger.Log("Dropping " + prefabName + " from " + invSection + ".");
			}
			GameManager.SetPrivateValue(inventory, "m_inventory", list);
		}

		[HarmonyPriority(Priority.VeryLow)]
		private static void Postfix(Player __instance, Dictionary<ItemDrop.ItemData, bool> __state)
		{
			Inventory inventory = GameManager.GetPrivateValue(__instance, "m_inventory") as Inventory;
			List<ItemDrop.ItemData> inventoryList = GameManager.GetPrivateValue(inventory, "m_inventory") as List<ItemDrop.ItemData>;
			foreach (KeyValuePair<ItemDrop.ItemData, bool> item in __state)
			{
				Logger.Log("Adding " + (item.Key.m_dropPrefab).name + " back to inventory.");
				inventoryList?.Add(item.Key);
				Logger.Log($"Item {item.Key.m_dropPrefab.name} was equipped: {item.Value}");
				if (item.Value && (!item.Key.m_dropPrefab.name.StartsWith("BBH") || !item.Key.m_dropPrefab.name.EndsWith("Quiver")))
				{
					__instance.UnequipItem(item.Key, false);
					__instance.EquipItem(item.Key, false);
				}
			}
			typeof(Inventory).GetMethod("Changed", BindingFlags.Instance | BindingFlags.NonPublic)?.Invoke(inventory, null);
		}
	}
}