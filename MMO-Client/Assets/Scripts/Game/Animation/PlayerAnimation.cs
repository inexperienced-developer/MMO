using InexperiencedDeveloper.Utils.Log;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : CharacterAnimation
{
    private Player m_Player;

    public virtual void Init()
    {
        base.Init();
        m_Player = GetComponent<Player>();
    }

    public override void SetAttack(int attack)
    {
        throw new System.NotImplementedException();
    }

    public override void SetJump(bool jump)
    {
        throw new System.NotImplementedException();
    }

    public override void SetMove(Vector3 move)
    {
        IDLogger.Log($"Right: {move.x} Forward: {move.z}");
        m_Anim.SetFloat("right", move.x);
        m_Anim.SetFloat("forward", move.z);
    }

    public override void SetMove(Vector2 move)
    {
        IDLogger.Log($"Right: {move.x} Forward: {move.y}");
        m_Anim.SetFloat("right", move.x);
        m_Anim.SetFloat("forward", move.y);
    }
}
