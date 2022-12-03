using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterAnimation : MonoBehaviour
{
    protected Animator m_Anim;

    public virtual void Init()
    {
        m_Anim = GetComponent<Animator>();
    }

    public abstract void SetMove(Vector2 move);
    public abstract void SetJump(bool jump);
    public abstract void SetAttack(int attack);
}
