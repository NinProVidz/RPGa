using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Miscellaneous Item")]
public class MiscellaneouscItemData : ItemData
{
    public int value;

    private void OnEnable()
    {
        itemType = ItemType.Miscellaneous; // or ItemType.Generic
    }
}
