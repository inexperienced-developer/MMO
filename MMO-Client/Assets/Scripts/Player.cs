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

    private void FixedUpdate()
    {
        if(IsLocal && Controls != null)
            SendInputs();
    }

    #region Messages
    //-------------------------------------------------------------------------------------//
    //---------------------------- Message Sending ----------------------------------------//
    //-------------------------------------------------------------------------------------//

    protected void SendInputs()
    {
        Message msg = Message.Create(MessageSendMode.Unreliable, (ushort)ClientToServerId.MoveRequest);
        Vector2 movement = new Vector2(Controls.Movement.x, Controls.Movement.z);
        float lookDir = transform.rotation.eulerAngles.y;
        bool jump = Controls.Jump;

        byte input = Utilities.BoolsToByte(new bool[7] { movement.x > 0, movement.x < 0, movement.y > 0, movement.y < 0, jump, Controls.RightClick, Controls.LeftClick });
        msg.AddByte(input);
        msg.AddFloatInt(Controls.PlayerMovement.LookDir);

        NetworkManager.Instance.Client.Send(msg);
    }

    #endregion
}
