using InexperiencedDeveloper.Utils.Log;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TreeTypes
{
    Normal = 0,
    Oak = 1,
    Maple = 2,
    Yew = 3,
}

public class Tree : HarvestObject
{
    [SerializeField] private TreeTypes m_Type;
    public TreeTypes Type => m_Type;

    public override void Interact(InGamePlayer player)
    {
        base.Interact(player);
        player.StateMachine.SetHarvestType(HarvestType.ChopTree);
        player.StateMachine.ChangeState(PlayerState.Harvesting);
    }

    public override void StopInteracting(InGamePlayer player)
    {
        base.StopInteracting(player);
        player.StateMachine.SetHarvestType(HarvestType.None);
        player.StateMachine.ChangeState(PlayerState.Idle);
    }
}
