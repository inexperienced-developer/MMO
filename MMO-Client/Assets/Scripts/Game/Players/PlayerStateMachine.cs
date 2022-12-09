using InexperiencedDeveloper.Utils.Log;
using Riptide;
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
    Mining,
}

public class PlayerStateMachine : MonoBehaviour
{
    private InGamePlayer m_Player;
    public PlayerState State { get; private set; }
    public HarvestType HarvestType { get; private set; }

    private bool m_AnimPlayed;
    private bool m_ShouldBeInteracting
    {
        get
        {
            //Add all interact states
            return State == PlayerState.Harvesting;
        }
    }

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
        IDLogger.Log($"Changed state from {State} to {newState}");
        State = newState;
        m_AnimPlayed = false;
        if (m_Player.IsLocal)
        {
            IDLogger.Log($"Interacting: {m_Player.Interacting}");
            IDLogger.Log($"Should be  Interacting: {m_ShouldBeInteracting}");
            if (m_Player.Interacting && !m_ShouldBeInteracting)
            {
                m_Player.StopInteracting();
                IDLogger.Log("Stopped Interacting");
            }
        }
        else
        {
            if(!m_ShouldBeInteracting)
                m_Player.StopInteracting();
        }
        m_Player.Anim.SetBool(Constants.ANIM_B_INTERACTING, m_ShouldBeInteracting);
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

        if(m_Player.IsLocal)
            SendHarvestType();
    }

    #region Messages
    //-------------------------------------------------------------------------------------//
    //---------------------------- Message Sending ----------------------------------------//
    //-------------------------------------------------------------------------------------//

    private void SendHarvestType()
    {
        Message msg = Message.Create(MessageSendMode.Reliable, ClientToServerId.SendHarvestType);
        IDLogger.Log($"Sending harvest type: {HarvestType}");
        msg.AddByte((byte)HarvestType);
        NetworkManager.Instance.Client.Send(msg);
    }

    #endregion
}
