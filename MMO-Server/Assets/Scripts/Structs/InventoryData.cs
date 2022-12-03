using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct InventoryData
{
    public List<Item> Items;
    public byte Size;

    public InventoryData(List<Item> items, byte size)
    {
        Items = items;
        Size = size;
    }
}
