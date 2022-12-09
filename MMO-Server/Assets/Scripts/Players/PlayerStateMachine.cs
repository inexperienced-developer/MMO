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
    private Player m_Player;
    public PlayerState State { get; private set; }
    public HarvestType HarvestType { get; private set; }

    private bool m_ShouldBeInteracting => State == PlayerState.Harvesting;
    //Harvest
    public HarvestObject CurrentHarvestObj { get; private set; }
    private float m_RunningHarvestTimer, m_CurrentHarvestTimer;

    public void Init()
    {
        m_Player = GetComponent<Player>();
    }

    protected void Update()
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
        State = newState;
        m_RunningHarvestTimer = 0;
        if (m_Player.Interacting && !m_ShouldBeInteracting)
            m_Player.StopInteract();
        PlayerManager.Instance.SendToNearbyPlayers(m_Player, SendState);
    }

    public void SetHarvestType(HarvestType newType)
    {
        HarvestType = newType;
        PlayerManager.Instance.SendToNearbyPlayers(m_Player, SendHarvestType);
        //List<Player> nearbyPlayers = PlayerManager.Instance.GetNearbyPlayers(transform.position);
        //foreach (Player player in nearbyPlayers)
        //{
        //    if (player == null) continue;
        //    SendHarvestType(player.Id);
        //}
    }

    private void Harvest()
    {
        if(CurrentHarvestObj == null && HarvestType == HarvestType.None)
        {
            IDLogger.LogError("No harvest object, shouldn't be in state");
            ChangeState(PlayerState.Idle);
            return;
        }
        if(CurrentHarvestObj != null && HarvestType == HarvestType.None)
        {
            IDLogger.LogWarning("Waiting for Harvest Type from Client");
            return;
        }
        m_RunningHarvestTimer += Time.deltaTime;
        if(m_RunningHarvestTimer >= m_CurrentHarvestTimer)
        {
            //Harvest
            ushort[] reward = CurrentHarvestObj.Harvest();
            m_Player.SendReward(m_Player.Id, reward);
            m_RunningHarvestTimer = 0;
        }
    }

    public void SetCurrentHarvestObj(HarvestObject obj)
    {
        CurrentHarvestObj = obj;
        m_RunningHarvestTimer = 0;
        m_CurrentHarvestTimer = obj.HarvestTimer;
    }

    //-------------------------------------------------------------------------------------//
    //---------------------------- Message Sending ----------------------------------------//
    //-------------------------------------------------------------------------------------//

    private void SendHarvestType(ushort toId)
    {
        Message msg = Message.Create(MessageSendMode.Reliable, ServerToClientId.HandleHarvestType);
        IDLogger.Log($"Sending Harvest Type: {HarvestType}");
        msg.AddUShort(m_Player.Id); // Who is harvesting
        msg.AddByte((byte)HarvestType); // Their type
        NetworkManager.Instance.Server.Send(msg, toId);
    }

    private void SendState(ushort toId)
    {

    }
}
