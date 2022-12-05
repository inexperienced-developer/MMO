using InexperiencedDeveloper.Core.Controls;
using InexperiencedDeveloper.Utils;
using InexperiencedDeveloper.Utils.Log;
using Riptide;
using System.Collections.Generic;
using UnityEngine;

public class InGamePlayer : Player
{
    public Gear Gear { get; private set; }
    public int GeoArea = -1;

    public PlayerMovement PlayerMovement { get; private set; }
    public PlayerAnimation Anim { get; protected set; }

    protected Vector3 lastPos;

    public override void Init(ushort id, string email = "")
    {
        base.Init(id, email);
        if (IsLocal)
        {
            if(Controls == null)
            {
                Controls = GetComponent<PlayerControls>() == null ? gameObject.AddComponent<PlayerControls>() : GetComponent<PlayerControls>();
            }

            Controls.Init();
            if (Inventory == null)
            {
                Inventory = GetComponent<Inventory>() == null ? gameObject.AddComponent<Inventory>() : GetComponent<Inventory>();
            }
            Inventory.Init();
        }
        if (PlayerMovement == null)
        {
            PlayerMovement = GetComponent<PlayerMovement>() == null ? gameObject.AddComponent<PlayerMovement>() : GetComponent<PlayerMovement>();
            PlayerMovement.Init();
        }
        if (Anim == null)
        {
            Anim = GetComponent<PlayerAnimation>() == null ? gameObject.AddComponent<PlayerAnimation>() : GetComponent<PlayerAnimation>();
            Anim.Init();
        }

    }

    private void FixedUpdate()
    {
        if (IsLocal) SendInputs();
    }

    public void ReceiveMovement(Vector3 pos, float yRot, bool[] inputs)
    {
        if(Id != NetworkManager.Instance.Client.Id)
        {
            PlayerMovement.SetInputs(yRot, inputs);
        }
        if (Vector3.Distance(pos, transform.position) > 1)
        {
            transform.position = pos;
        }
    }

    #region Messages
    //-------------------------------------------------------------------------------------//
    //---------------------------- Message Sending ----------------------------------------//
    //-------------------------------------------------------------------------------------//

    protected void SendInputs()
    {
        Message msg = Message.Create(MessageSendMode.Unreliable, (ushort)ClientToServerId.MoveRequest);
        Vector2 movement = PlayerMovement.MoveDir;
        bool jump = Controls.Jump;

        byte input = Utilities.BoolsToByte(new bool[7] { movement.x > 0, movement.x < 0, movement.y > 0, movement.y < 0, jump, Controls.RightClick, Controls.LeftClick });
        msg.AddByte(input);
        msg.AddFloatInt(Controls.PlayerMovement.LookDir);

        NetworkManager.Instance.Client.Send(msg);
    }

    #endregion

}
