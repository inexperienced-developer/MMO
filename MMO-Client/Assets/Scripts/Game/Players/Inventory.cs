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
        foreach(var bag in m_Bags)
        {
            if (bag!= null && bag.HasAxe)
            {
                HasAxe = true;
                break;
            }
        }
    }

    public void SetBags(List<Bag> bags)
    {
        IDLogger.Log($"Added bags {bags.Count}");
        for(int i = 0; i < bags.Count; i++)
        {
            m_Bags[i] = bags[i];
        }
        Init();
    }
}
