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
        m_Anim.SetFloat(Constants.ANIM_RIGHT, move.x);
        m_Anim.SetFloat(Constants.ANIM_FWD, move.z);
    }

    public override void SetMove(Vector2 move)
    {
        m_Anim.SetFloat(Constants.ANIM_RIGHT, move.x);
        m_Anim.SetFloat(Constants.ANIM_FWD, move.y);
    }

    public void SetBool(string name, bool start)
    {
        m_Anim.SetBool(name, start);
    }

    public void PlayTrigger(string name)
    {
        m_Anim.SetTrigger(name);
    }
}
