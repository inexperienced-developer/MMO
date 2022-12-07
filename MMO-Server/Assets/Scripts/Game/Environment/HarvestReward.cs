using System;
using UnityEngine;

[Serializable]
public class HarvestReward
{
    [SerializeField] private Item m_Item;
    public Item Item => m_Item;
    [SerializeField] private ushort[] m_QuantityRange = new ushort[2];
    public ushort[] QuantityRange => m_QuantityRange;
    [SerializeField] private float m_Probability;
    public float Probability => m_Probability;

    public HarvestReward(Item item, ushort[] quanityRange, float probability)
    {
        m_Item = item;
        m_QuantityRange = quanityRange;
        m_Probability = probability;
    }
}
