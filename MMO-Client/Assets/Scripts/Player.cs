using InexperiencedDeveloper.Core.Controls;
using InexperiencedDeveloper.Utils;
using InexperiencedDeveloper.Utils.Log;
using Riptide;
using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    public ushort Id { get; protected set; }
    public bool IsLocal { get; protected set; }
    protected string m_Email;
    public string Email => m_Email;
    public PlayerControls Controls { get; protected set; }
    public bool DEBUG = false;

    protected virtual void OnDestroy()
    {
        PlayerManager.RemovePlayerFromList(this);
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }

    public virtual void Init(ushort id, string email = "")
    {
        Id = id;
        m_Email = email;
        if (Id == NetworkManager.Instance.Client.Id)
        {
            IsLocal = true;
        }
        else
        {
            IsLocal = false;
        }
    }
}
