using UnityEngine;

public enum ArmorType
{
    Leather = 1,
    Copper = 2,
    Bronze = 3,
    Silver = 4,
    Gold = 5,
    Mithril = 6,
    Rune = 7
}

[CreateAssetMenu(menuName = "Items/Armor")]
public class Armor : EquippableItem
{
    public ArmorType ArmorType;
    public byte Defense;
}
