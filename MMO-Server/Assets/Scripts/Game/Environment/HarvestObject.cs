using InexperiencedDeveloper.Utils.Log;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class HarvestObject : MonoBehaviour, IInteractable, IDamageable
{
    [SerializeField] protected byte m_InteractSlots = 1;
    [SerializeField] protected float m_HarvestTimer = 5;
    [SerializeField] protected HarvestReward[] m_HarvestRewards;
    [SerializeField] protected ushort m_DamagePerTick = 5;
    [SerializeField] protected InteractType m_InteractType;
    public float HarvestTimer => m_HarvestTimer;
    protected Player[] m_Interactors;

    public bool Full
    {
        get
        {
            foreach (var a in m_Interactors)
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
        m_Interactors = new Player[m_InteractSlots];
    }

    //protected void OnTriggerEnter(Collider other)
    //{
    //    if (Full) return;
    //    Player player = other.GetComponent<Player>();
    //    if (player != null) player.SetInteractable(this);
    //}

    //protected void OnTriggerExit(Collider other)
    //{
    //    InGamePlayer player = other.GetComponent<InGamePlayer>();
    //    if (player != null) player.SetInteractable(null);
    //}

    public virtual void Interact(Player player)
    {
        if (Full)
        {
            IDLogger.LogError($"Object is full.");
            return;
        }
        for (int i = 0; i < m_Interactors.Length; i++)
        {
            if (m_Interactors[i] == null)
            {
                m_Interactors[i] = player;
                break;
            }
        }
        player.StateMachine.SetCurrentHarvestObj(this);
        player.StateMachine.ChangeState(PlayerState.Harvesting);
    }

    public virtual void StopInteracting(Player player)
    {
        for (int i = 0; i < m_Interactors.Length; i++)
        {
            if (m_Interactors[i] == player)
            {
                m_Interactors[i] = null;
                break;
            }
        }
        player.StateMachine.ChangeState(PlayerState.Idle);
        player.StopInteract();
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }


    public InteractType GetInteractType()
    {
        return m_InteractType;
    }

    public void SendItem(Player player)
    {
        if (m_Interactors.Contains(player))
        {
            //player.Inventory.
        }
    }

    public virtual ushort[] Harvest()
    {
        var probabilityOrdered = m_HarvestRewards.OrderBy(x => x.Probability).ToList();
        ushort[] item = new ushort[2];
        for (int i = 0; i < probabilityOrdered.Count; i++)
        {
            float roll = Random.Range(0f, 1f);
            if(roll <= probabilityOrdered[i].Probability || i == probabilityOrdered.Count - 1)
            {
                item[0] = probabilityOrdered[i].Item.Id;
                item[1] = (ushort)Random.Range(probabilityOrdered[i].QuantityRange[0], probabilityOrdered[i].QuantityRange[1] + 1);
                TakeDamage(m_DamagePerTick);
                break;
            }
        }
        // [ItemId, Quantity]
        return item;
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
