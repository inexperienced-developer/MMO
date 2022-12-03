using InexperiencedDeveloper.Utils.Log;
using UnityEngine;

public enum WeaponSlot : byte
{
    OneHand = 1,
    TwoHand = 2,
    MainHand = 3,
    OffHand = 4,
}

public enum WeaponType : byte
{
    Sword = 1,
    Dagger = 2,
    Axe = 3,
    Hammer = 4,
}


[CreateAssetMenu(menuName = "Items/Weapon")]
public class Weapon : EquippableItem
{
    public WeaponType WeaponType;
    public WeaponSlot WeaponSlot;
    public ushort Damage;
    public byte Speed;

    public void Attack(IDamageable target)
    {
        target.TakeDamage(Damage);
        IDLogger.Log($"Damaged {target}");
    }
}
