using InexperiencedDeveloper.Utils.Log;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState : byte
{
    Idle = 0,
    Moving,
    Harvesting,
}

public enum HarvestType : byte
{
    None = 0,
    ChopTree,
    Mining
}

public class PlayerStateMachine : MonoBehaviour
{
    private InGamePlayer m_Player;
    public PlayerState State { get; private set; }
    public HarvestType HarvestType { get; private set; }

    private bool m_AnimPlayed;

    public void Init()
    {
        m_Player = GetComponent<InGamePlayer>();
        ChangeState(PlayerState.Idle);
    }

    private void Update()
    {
        switch (State)
        {
            case PlayerState.Idle:
                break;
            case PlayerState.Moving:
                Moving();
                break;
            case PlayerState.Harvesting:
                Harvest();
                break;
            default:
                break;
        }
    }

    public void ChangeState(PlayerState newState)
    {
        if (newState == PlayerState.Harvesting && State == PlayerState.Moving)
        {
            IDLogger.LogWarning("Cannot harvest while moving.");
            return;
        }
        State = newState;
        m_AnimPlayed = false;
    }

    private void Moving()
    {
        if(m_Player.Interacting) m_Player.StopInteracting();
    }

    public void SetHarvestType(HarvestType newType)
    {
        HarvestType = newType;
    }

    private void Harvest()
    {
        PlayHarvestAnim();
    }

    private void PlayHarvestAnim()
    {
        if (m_AnimPlayed) return;
        if (HarvestType == HarvestType.None) ChangeState(PlayerState.Idle);
        string trigger = "";
        switch (HarvestType)
        {
            case HarvestType.ChopTree:
                trigger = Constants.ANIM_T_TREE;
                break;
            case HarvestType.Mining:
                trigger = Constants.ANIM_T_MINING;
                break;
            default:
                break;
        }
        m_Player.Anim.PlayTrigger(trigger);
        m_AnimPlayed = true;
    }
}
