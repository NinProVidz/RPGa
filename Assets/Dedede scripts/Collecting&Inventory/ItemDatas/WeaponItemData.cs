using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Weapon Item")]
public class WeaponItemData : ItemData
{
    public int value;
    public float damage;

    private void OnEnable()
    {
        itemType = ItemType.Weapon;
    }

    public override string GetMainDesc()
    {
        string mainDesc = mainDescription;
        return mainDescription.Replace("{damage}", damage.ToString());
    }
}
