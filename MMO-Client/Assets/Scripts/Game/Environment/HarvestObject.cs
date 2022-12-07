using InexperiencedDeveloper.Utils.Log;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HarvestObject : MonoBehaviour, IInteractable, IDamageable
{
    [SerializeField] private Sprite m_Sprite;
    public Sprite Sprite => m_Sprite;

    [SerializeField] protected byte m_InteractSlots = 1;
    protected InGamePlayer[] m_Interactors;
    public bool Full
    {
        get
        {
            foreach(var a in m_Interactors)
            {
                if (a == null) return false;
            }
            return true;
        }
    }

    [SerializeField] protected ushort m_Health;
    public ushort Health => m_Health;

    public bool IsReady { get; protected set; }

    protected virtual void Awake()
    {
        m_Interactors = new InGamePlayer[m_InteractSlots];
    }

    protected void OnTriggerEnter(Collider other)
    {
        if (Full) return;
        InGamePlayer player = other.GetComponent<InGamePlayer>();
        if (player != null) player.SetInteractable(this);
    }

    protected void OnTriggerExit(Collider other)
    {
        InGamePlayer player = other.GetComponent<InGamePlayer>();
        if (player != null) player.SetInteractable(null);
    }

    public virtual void Interact(InGamePlayer player)
    {
        if (Full)
        {
            IDLogger.LogError($"Object is full.");
            return;
        }
        for(int i = 0; i < m_Interactors.Length; i++)
        {
            if (m_Interactors[i] == null)
            {
                m_Interactors[i] = player;
                break;
            }
        }
        player.Anim.SetBool(Constants.ANIM_B_INTERACTING, true);
    }

    public virtual void StopInteracting(InGamePlayer player)
    {
        for (int i = 0; i < m_Interactors.Length; i++)
        {
            if (m_Interactors[i] == player)
            {
                m_Interactors[i] = null;
                break;
            }
        }
        player.Anim.SetBool(Constants.ANIM_B_INTERACTING, false);
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public void SendItem(InGamePlayer player)
    {
        if (m_Interactors.Contains(player))
        {
            //player.Inventory.
        }
    }

    public virtual void TakeDamage(ushort damage)
    {
        m_Health -= damage;
        if (m_Health <= 0) Die();
    }

    public virtual void Die()
    {
        foreach (var p in m_Interactors)
            StopInteracting(p);
        IsReady = false;
    }


}
