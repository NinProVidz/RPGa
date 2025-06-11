using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public struct ItemTypeInventoryPair
{
    public ItemType itemType;
    public SubInventory inventory;
}

[System.Serializable]
public class SubInventory
{
    public List<InventoryItem> subInventory = new List<InventoryItem>();
    public Dictionary<ItemData, InventoryItem> itemDictionary = new Dictionary<ItemData, InventoryItem>();
}

public class Inventory : MonoBehaviour
{
    public float sigma;
    public static event Action<List<InventoryItem>> OnInventoryChange;

    public List<ItemTypeInventoryPair> itemTypeInventories;
    private Dictionary<ItemType, SubInventory> itemTypeInventoryDict;

    //public Dictionary<ItemData, InventoryItem> itemDictionary = new Dictionary<ItemData, InventoryItem>();

    private void Awake()
    {
        itemTypeInventoryDict = new Dictionary<ItemType, SubInventory>();
        foreach (var pair in itemTypeInventories)
        {
            itemTypeInventoryDict[pair.itemType] = pair.inventory;
        }
    }

    private void OnEnable()
    {
        Items.OnItemCollected += Add;
    }

    private void OnDisable()
    {
        Items.OnItemCollected -= Add;
    }

    public void Add(ItemData itemData)
    {
        if(itemTypeInventoryDict.TryGetValue(itemData.itemType, out SubInventory inventory))
        {
            if (inventory.itemDictionary.TryGetValue(itemData, out InventoryItem item))
            {
                item.AddToStack();
                Debug.Log($"{item.itemData.displayName} total stack is now {item.stackSize}");
                OnInventoryChange?.Invoke(inventory.subInventory);
            }
            else
            {
                InventoryItem newItem = new InventoryItem(itemData);
                inventory.subInventory.Add(newItem);
                inventory.itemDictionary.Add(itemData, newItem);
                Debug.Log($"Added {newItem.itemData.displayName} to the inventory for the first time.");
                OnInventoryChange?.Invoke(inventory.subInventory);
            }
        }
    }

    public void Remove(ItemData itemData)
    {
        if (itemTypeInventoryDict.TryGetValue(itemData.itemType, out SubInventory inventory))
        {
            if (inventory.itemDictionary.TryGetValue(itemData, out InventoryItem item))
            {
                item.RemoveFromStack();
                if (item.stackSize == 0)
                {
                    inventory.subInventory.Remove(item);
                    inventory.itemDictionary.Remove(itemData);
                }
                OnInventoryChange?.Invoke(inventory.subInventory);
            }
        }
    }
}
