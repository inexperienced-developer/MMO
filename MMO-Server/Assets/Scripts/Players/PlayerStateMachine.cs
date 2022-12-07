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

public class PlayerStateMachine : MonoBehaviour
{
    private Player m_Player;
    public PlayerState State { get; private set; }

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
    }

    private void Harvest()
    {
        if(CurrentHarvestObj == null)
        {
            IDLogger.LogError("No harvest object");
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
}
