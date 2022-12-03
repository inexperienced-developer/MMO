using System.Collections.Generic;

public struct GearData
{
    private List<EquippableItem> m_EquippedItems;

    public GearData(List<EquippableItem> items)
    {
        m_EquippedItems = items;
    }
}
