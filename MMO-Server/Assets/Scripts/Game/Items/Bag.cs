using InexperiencedDeveloper.Utils.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class Bag
{

    [SerializeField] private ushort m_ItemId;
    [SerializeField] private string m_Name;
    [SerializeField] private BagCategories m_BagType;
    [SerializeField] private Sprite m_Icon;
    [SerializeField] private Item[] m_Items;
    private List<int> m_EmptySlots = new List<int>();
    public ushort ItemId => m_ItemId;
    public string Name => m_Name;
    public BagCategories BagType => m_BagType;
    public Sprite Icon => m_Icon;
    public Item[] Items => m_Items;
    public bool IsFull => m_EmptySlots.Count <= 0;
    public bool HasAxe
    {
        get
        {
            foreach (var item in m_Items)
            {
                if (item != null)
                {
                    if (item.GetType() == typeof(Weapon))
                    {
                        Weapon weapon = (Weapon)item;
                        if (weapon.WeaponType == WeaponType.Axe && weapon.WeaponSlot == WeaponSlot.OneHand)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }

    public Bag (ushort itemId, string name, byte numOfSlots, BagCategories bagType, Sprite icon)
    {
        m_ItemId = itemId;
        m_Name = name;
        m_BagType = bagType;
        m_Icon = icon;
        m_Items = new Item[numOfSlots];
        for(int i = 0; i < numOfSlots; i++)
        {
            m_EmptySlots.Add(i);
        }
    }

    public bool AddItem(Item item)
    {
        if (m_EmptySlots.Count <= 0)
        {
            IDLogger.LogWarning("Bag is full");
            return false;
        }
        m_Items[m_EmptySlots[0]] = item;
        m_EmptySlots.Remove(0);
        return true;
    }

    public bool RemoveItem(int index)
    {
        if (m_EmptySlots.Contains(index) || m_Items[index] == null)
        {
            IDLogger.LogWarning("Slot is empty");
            return false;
        }
        m_Items[index] = null;
        m_EmptySlots.Add(index);
        m_EmptySlots = m_EmptySlots.OrderBy(x => x).ToList();
        return true;
    }
}
