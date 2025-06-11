using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public struct ItemTypeColorPair
{
    public ItemTier itemTier;
    public Color color;
}

public class InventoryManager : MonoBehaviour
{
    public ItemType inventoryType;

    public GameObject slotPrefab;
    public List<InventorySlot> inventorySlot = new List<InventorySlot>(20);

    public InventorySlot selectedSlot;

    [Header("Item Type Colors")]
    public List<ItemTypeColorPair> itemTypeColors;

    private Dictionary<ItemTier, Color> itemTypeColorDict;

    

    private void Awake()
    {
        itemTypeColorDict = new Dictionary<ItemTier, Color>();
        foreach (var pair in itemTypeColors)
        {
            itemTypeColorDict[pair.itemTier] = pair.color;
        }
    }


    private void OnEnable()
    {
        Inventory.OnInventoryChange += DrawInventory;
    }

    private void OnDisable()
    {
        Inventory.OnInventoryChange -= DrawInventory;
    }

    void ResetInventory()
    {
        foreach(Transform childTransform in transform)
        {
            Destroy(childTransform.gameObject);
        }
        inventorySlot = new List<InventorySlot>(20);
    }

    void DrawInventory(ItemType itemType, SubInventory inventory)
    {
        if (inventoryType == itemType)
        {
            ResetInventory();
            for (int i = 0; i < inventory.subInventory.Count; i++)
            {
                CreateInventorySlot();
            }
            for (int i = 0; i < inventory.subInventory.Count; i++)
            {
                inventorySlot[i].DrawSlot(inventory.subInventory[i]);
                inventorySlot[i].item = inventory.subInventory[i];

                ItemTier type = inventory.subInventory[i].itemData.itemTier;

                inventorySlot[i].itemColor = itemTypeColorDict.GetValueOrDefault(type, Color.white);
                //inventorySlot[i].enabled = true;
            }
        }
    }

    void CreateInventorySlot()
    {
        GameObject newSlot = Instantiate(slotPrefab);
        newSlot.transform.SetParent(transform, false);

        InventorySlot newSlotComponent = newSlot.GetComponent<InventorySlot>();
        newSlotComponent.ClearSlot();
        newSlotComponent.inventoryManager = this;

        inventorySlot.Add(newSlotComponent);
    }

    public void SelectSlot(InventorySlot selectedSlot)
    {

        if (this.selectedSlot != null)
        {
            this.selectedSlot.isSelected = false;
        }

        if(this.selectedSlot == selectedSlot)
        {
            this.selectedSlot = null;
        }
        else
        {
            selectedSlot.isSelected = true;
            this.selectedSlot = selectedSlot;
        }
        
    }
}
