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
    void Interact(InGamePlayer player);
    void StopInteracting(InGamePlayer player);
    Vector3 GetPosition();
}