using InexperiencedDeveloper.Core.Controls;
using InexperiencedDeveloper.Utils;
using InexperiencedDeveloper.Utils.Log;
using Riptide;
using System.Collections.Generic;
using UnityEngine;

public enum InteractType : byte
{
    None = 0,
    Harvest,
}

public class InGamePlayer : Player
{
    public Gear Gear { get; private set; }
    public int GeoArea = -1;

    public PlayerMovement PlayerMovement { get; private set; }
    public PlayerInteraction PlayerInteraction { get; private set; }
    public PlayerAnimation Anim { get; protected set; }
    public PlayerStateMachine StateMachine { get; protected set; }

    protected Vector3 lastPos;

    protected LayerMask m_NonPlayerLayer;

    protected IInteractable m_LastInteractable;
    protected IInteractable m_CurrentInteractable;
    public bool Interacting { get; protected set; }

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
            if (PlayerInteraction == null)
            {
                PlayerInteraction = GetComponent<PlayerInteraction>() == null ? gameObject.AddComponent<PlayerInteraction>() : GetComponent<PlayerInteraction>();
            }
            PlayerInteraction.Init();
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
        if(StateMachine == null)
        {
            StateMachine = GetComponent<PlayerStateMachine>() == null ? gameObject.AddComponent<PlayerStateMachine>() : GetComponent<PlayerStateMachine>();
        }
        StateMachine.Init();
        m_NonPlayerLayer = PlayerManager.Instance.NonPlayerLayer;
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

    public void SetInteractable(IInteractable interactable)
    {
        m_LastInteractable = interactable;
    }

    public void Interact(IInteractable interactable)
    {
        IDLogger.LogError($"Interactable: {interactable}");
        if(interactable != null)
        {
            Vector3 pos = transform.position;
            pos.y += 1;
            RaycastHit hit;
            Debug.DrawRay(pos, transform.forward, Color.red, 10);
            if (Physics.Raycast(pos, transform.forward, out hit, Constants.PLAYER_INTERACT_DISTANCE, m_NonPlayerLayer))
            {
                if(hit.collider.GetComponent<IInteractable>() == interactable)
                {
                    interactable.Interact(this);
                    InteractRequestToServer(interactable.GetPosition());
                    Interacting = true;
                    Anim.SetBool(Constants.ANIM_B_INTERACTING, true);
                    m_CurrentInteractable = interactable;
                }
            }
            else
            {
                IDLogger.LogError($"Must be facing {interactable} to interact with it");
            }
        }
    }

    public void StopInteracting()
    {
        Anim.SetBool(Constants.ANIM_B_INTERACTING, false);
        if (m_CurrentInteractable == null) return;
        IInteractable interactable = m_CurrentInteractable;
        m_CurrentInteractable = null;
        interactable.StopInteracting(this);
        Interacting = false;
    }

    public void ReceiveHarvestReward(ushort[] reward)
    {
        string itemId = reward[0].ToString(Constants.ITEM_ID_FORMAT);
        if (ItemManager.ItemDict.TryGetValue(itemId, out Item item))
        {
            Inventory.AddItem(item, reward[1], false);
        }
        else
        {
            IDLogger.LogError($"Can't find item with id {itemId}");
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

        byte input = Utilities.BoolsToByte(new bool[7] { movement.x > 0, movement.x < 0, movement.y > 0, movement.y < 0, jump, Controls.RightClickHeld, Controls.LeftClick });
        msg.AddByte(input);
        msg.AddFloatInt(Controls.PlayerMovement.LookDir);

        NetworkManager.Instance.Client.Send(msg);
    }

    protected void InteractRequestToServer(Vector3 targetPos)
    {
        Message msg = Message.Create(MessageSendMode.Reliable, ClientToServerId.RequestInteract);
        msg.AddVector3Int(targetPos);
        NetworkManager.Instance.Client.Send(msg);
    }

    #endregion

}
