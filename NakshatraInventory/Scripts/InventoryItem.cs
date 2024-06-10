using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory Item", menuName = "Inventory/Item")]
public class InventoryItem : ScriptableObject
{
    public string itemName = "New Item";
    public string itemDescription = "Item Description";
    public Sprite itemIcon = null;
    public bool isStackable = false;
    public int maxStackSize = 1;

    public ItemType itemType;
    public int amount = 0;  // For consumables like health potions

    // Dynamic stats
    public List<ItemStat> stats = new List<ItemStat>();
}

public enum ItemType
{
    Consumable,
    Equipment,
    Other
}

[System.Serializable]
public class ItemStat
{
    public StatType statType;
    public int value;
}

public enum StatType
{
    Attack,
    Defense,
    Block,
    Intelligence,
    Health,
    Mana,
    Stamina,
    Speed,
    Agility,
    Strength,
    Dexterity,
    Luck
}
