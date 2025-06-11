using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ItemType
{
    Weapon,
    Consumable,
    Armor,
    Material,
    Miscellaneous
}

public enum ItemTier
{
    Common,
    Notable,
    Valuable,
    Exceptional,
    Special
}

public abstract class ItemData : ScriptableObject
{
    public string displayName;
    public Sprite Icon;
    public ItemType itemType;
    public ItemTier itemTier;
    [TextArea()]
    public string description;
}




