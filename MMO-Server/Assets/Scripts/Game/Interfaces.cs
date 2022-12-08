using UnityEngine;

public interface IUseable
{
    void Use();
}

public interface IDamageable
{
    void TakeDamage(ushort damage);
    void Die();
}

public interface IInteractable
{
    void Interact(Player player);
    void StopInteracting(Player player);
    Vector3 GetPosition();
}