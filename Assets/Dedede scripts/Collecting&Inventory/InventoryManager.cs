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

    public ItemInfoDisplay itemInfoDisplay;

    private InventorySlot selectedSlot;

    public InventorySlot SelectedSlot
    {
        get => selectedSlot;
        set
        {
            if (selectedSlot != value)
            {
                selectedSlot = value;
                if (itemInfoDisplay != null)
                {
                    itemInfoDisplay.DisplayItemInfo(selectedSlot); // Call method only when set to a new non-null slot
                }
            }
        }
    }

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

    private void Start()
    {
    }


    private void OnEnable()
    {
        DrawInventory(inventoryType, Inventory.itemTypeInventoryDict.GetValueOrDefault(inventoryType));
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

        if (SelectedSlot != null)
        {
            SelectedSlot.isSelected = false;
        }

        if(SelectedSlot == selectedSlot)
        {
            SelectedSlot = null;
        }
        else
        {
            selectedSlot.isSelected = true;
            SelectedSlot = selectedSlot;
        }
        
    }
}
