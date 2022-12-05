using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryData
{
    public List<ushort> BagIDs;
    public List<ushort> ItemIDs;

    public InventoryData()
    {
        ItemIDs = new List<ushort>();
        BagIDs = new List<ushort>();
    }

    public InventoryData(List<ushort> bagIDs, List<ushort> itemIDs)
    {
        BagIDs = bagIDs;
        ItemIDs = itemIDs;
    }
}
