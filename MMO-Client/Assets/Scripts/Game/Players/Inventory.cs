using InexperiencedDeveloper.Utils.Log;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] protected Bag[] m_Bags = new Bag[5];

    public bool HasAxe { get; private set; }

    public void Init()
    {
        IDLogger.Log("Created Inventory");
        foreach (var bag in m_Bags)
        {
            if (bag != null && bag.HasAxe)
            {
                HasAxe = true;
                break;
            }
        }
    }

    public void SetBags(List<Bag> bags)
    {
        IDLogger.Log($"Added bags {bags.Count}");
        for (int i = 0; i < bags.Count; i++)
        {
            m_Bags[i] = bags[i];
        }
        Init();
    }

    public void AddItem(Item item, ushort quantity, bool stacks)
    {
        ushort q = quantity;
        foreach(var bag in m_Bags)
        {
            if (bag == null || bag.IsFull) continue;
            for(int i = 0; i < bag.Items.Length; i++)
            {
                if (bag.Items[i] != null) continue;
                bag.Items[i] = item;
                q--;
                if (q <= 0) return;
            }
        }
    }
}
