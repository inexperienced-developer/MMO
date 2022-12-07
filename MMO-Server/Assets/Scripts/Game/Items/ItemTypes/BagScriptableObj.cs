using InexperiencedDeveloper.Utils.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum BagCategories : byte
{
    None = 0,
    Everything = Wood | Herbs, 
    Wood = 1 << 0,
    Herbs = 2 << 1,
}

[CreateAssetMenu(menuName ="Items/Bag")]
public class BagScriptableObj : Item
{
    [SerializeField] protected int m_NumSlots;
    [SerializeField] protected BagCategories m_Categories;
    public int NumSlots => m_NumSlots;
    public BagCategories Categories => m_Categories;
}
