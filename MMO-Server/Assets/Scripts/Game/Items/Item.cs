using System;

public enum Slot : byte
{
    Unequippable = 0,
    WeaponOneHand = 1,
    WeaponTwoHand = 2,
    OffHand = 3,
    Head = 4,
    Neck = 5,
    Shoulders = 6,
    Chest = 7,
    Belt = 8,
    Pants = 9,
    Boots = 10,
    Ring = 11,
    Bag = 12,
}

[Serializable]
public class Item
{
    public ushort Id;
    public string ItemName;
    public Slot Slot;

    public virtual void Use()
    {
        return;
    }

    public Item(ushort id, string itemName, Slot slot)
    {
        Id = id;
        ItemName = itemName;
        Slot = slot;
    }
}
