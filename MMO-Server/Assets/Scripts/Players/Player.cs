using InexperiencedDeveloper.MMO.Data;
using InexperiencedDeveloper.Utils;
using InexperiencedDeveloper.Utils.Log;
using Riptide;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class Player : MonoBehaviour
{
    public ushort Id { get; private set; }
    public string Email { get; private set; }
    public List<CharacterAppearanceData> Characters { get; private set; }
    public CharacterAppearanceData SpawnedCharacter { get; private set; }
    public InventoryData CurrentInventoryData { get; private set; }
    public GearData CurrentGear { get; private set; }
    public PlayerMovement PlayerMovement { get; private set; }
    public CharacterController Controller { get; private set; }
    public PlayerStateMachine StateMachine { get; private set; }
    public bool Interacting { get; private set; }

    private IInteractable m_CurrentInteractable;

    private void OnDestroy()
    {
        PlayerManager.RemovePlayerFromList(this);
    }

    public void Init(ushort id, string username, List<CharacterAppearanceData> data)
    {
        Id = id;
        Email = username;
        Characters = data;
    }

    public void InitSpawn(Vector3 pos, Quaternion rot)
    {
        PlayerMovement = PlayerMovement == null ? gameObject.AddComponent<PlayerMovement>() : PlayerMovement;
        PlayerMovement.Init();
        Controller = GetComponent<CharacterController>();
        Controller.center = Constants.CHARACTER_CONTROLLER_CAPSULE_CENTER;
        transform.position = pos;
        transform.rotation = rot;
        StateMachine = StateMachine == null ? gameObject.AddComponent<PlayerStateMachine>() : StateMachine;
        StateMachine.Init();
    }


    public void SetSpawnedCharacter(string n)
    {
        foreach(var c in Characters)
        {
            if(c.Name.ToLower() == n.ToLower())
            {
                SpawnedCharacter = c;
                break;
            }
        }
    }

    public void SetInventoryData(InventoryData inventory)
    {
        CurrentInventoryData = inventory;
    }

    public void Interact(Vector3 interactPos)
    {
        //RaycastHit[] hits = Physics.RaycastAll(transform.position, transform.forward, 5, m_NonPlayerLayer);
        Collider[] hits = Physics.OverlapSphere(interactPos, 1, PlayerManager.Instance.NonPlayerLayer, QueryTriggerInteraction.Ignore);
        if (hits.Length > 0)
        {
            foreach(var hit in hits)
            {
                IInteractable obj = hit.GetComponent<IInteractable>() == null ? hit.GetComponentInParent<IInteractable>(true) : hit.GetComponent<IInteractable>();
                if (obj != null)
                {
                    m_CurrentInteractable = obj;
                    Interacting = true;
                    obj.Interact(this);
                    break;
                }
            }
        }
    }

    public void StopInteract()
    {
        if (m_CurrentInteractable == null) return;
        IInteractable i = m_CurrentInteractable;
        m_CurrentInteractable = null;
        Interacting = false;
        i.StopInteracting(this);
    }

    //public async Task SetGear(List<float> gearData)
    //{
    //    List<EquippableItem> items = new List<EquippableItem>();
    //    try
    //    {
    //        foreach (var item in gearData)
    //        {
    //            ushort id = (ushort)item;
    //            string itemStr = await ItemManager.GetItemString(id);
    //            string[] itemDetails = itemStr.Split("`");
    //            byte slot = byte.Parse(itemDetails[1]);
    //            bool hasAbility = bool.Parse(itemDetails[2]);
    //            EquippableItem newItem = new EquippableItem(id, itemDetails[0], (Slot)slot, true, hasAbility);
    //            items.Add(newItem);
    //        }
    //        CurrentGear = new GearData(items);
    //    }
    //    catch (System.Exception e)
    //    {
    //        IDLogger.LogError($"Failed building gear with Exception: {e}");
    //    }
    //}

    //-------------------------------------------------------------------------------------//
    //---------------------------- Message Sending ----------------------------------------//
    //-------------------------------------------------------------------------------------//

    public void SendCharacterList(List<CharacterAppearanceData> data)
    {
        Message msg = Message.Create(MessageSendMode.Reliable, (ushort)ServerToClientId.AccountCharacterData);
        //Send length of list
        msg.AddUShort(Id);
        msg.AddString(Email);
        msg.AddByte((byte)data.Count);
        for(int i = 0; i < data.Count; i++)
        {
            msg = AddCharacterData(msg, data[i]);
        }
        NetworkManager.Instance.Server.Send(msg, Id);
    }

    //Send to 1 player
    public void SendSpawned(ushort toId, Vector3 pos, Quaternion rot, List<float> gear, byte geoArea, ushort aboutId)
    {
        Message msg = Message.Create(MessageSendMode.Reliable, (ushort)ServerToClientId.SpawnPlayer);
        msg = AddSpawnData(msg, aboutId, pos, rot, gear, geoArea);
        msg = AddCharacterData(msg, SpawnedCharacter);
        NetworkManager.Instance.Server.Send(msg, toId);
    }

    //Send to all players
    public void SendSpawned(Vector3 pos, Quaternion rot, List<float> gear, byte geoArea, ushort aboutId)
    {
        Message msg = Message.Create(MessageSendMode.Reliable, (ushort)ServerToClientId.SpawnPlayer);
        msg = AddSpawnData(msg, aboutId, pos, rot, gear, geoArea);
        msg = AddCharacterData(msg, SpawnedCharacter);
        NetworkManager.Instance.Server.SendToAll(msg);
    }

    private Message AddSpawnData(Message msg, ushort id, Vector3 pos, Quaternion rot, List<float> gear, byte geoArea)
    {
        msg.AddUShort(id);
        msg.AddByte(geoArea);
        msg.AddVector3Int(pos);
        msg.AddQuaternionInt(rot);
        msg.AddByte((byte)gear.Count); //Length of gear
        foreach (var item in gear)
        {
            msg.AddUShort((ushort)item);
        }
        return msg;
    }

    public void SendInventory(ushort toId, InventoryData inventory)
    {
        Message msg = Message.Create(MessageSendMode.Reliable, (ushort)ServerToClientId.SendInventory);
        msg.AddByte((byte)inventory.BagIDs.Count); //Length of Bags
        //Send bags
        for (int i = 0; i < inventory.BagIDs.Count; i++)
        {
            msg.AddUShort(inventory.BagIDs[i]);
        }
        msg.AddByte((byte)inventory.ItemIDs.Count); //Size of inventory
        //Add each inventory item
        for (int i = 0; i < inventory.ItemIDs.Count; i++)
        {
            msg.AddUShort(inventory.ItemIDs[i]);
        }
        NetworkManager.Instance.Server.Send(msg, toId);
    }
    private Message AddCharacterData(Message msg, CharacterAppearanceData data)
    {
        msg.AddString(data.Name);
        msg.AddByte(data.Level);
        msg.AddUShort(data.TotalLevel);
        msg.AddByte(data.SkinColor);
        msg.AddByte(data.HairColor);
        msg.AddByte(data.HairStyle);
        msg.AddByte(data.FacialHairStyle);
        msg.AddByte(data.EyebrowStyle);
        msg.AddByte(data.EyeColor);
        byte appearanceBools = CreateAppearanceBools(data);
        msg.AddByte(appearanceBools);
        return msg;
    }

    private byte CreateAppearanceBools(CharacterAppearanceData data)
    {
        // Pants << 0
        // Shirt << 1
        // Boots << 2
        bool[] appearanceBools = new bool[3] { data.PantsOn, data.ShirtOn, data.BootsOn };
        return Utilities.BoolsToByte(appearanceBools);
    }

    private Message AddSpawnData(Message msg)
    {
        msg.AddUShort(Id);
        msg.AddString(Email);
        msg.AddVector3Int(transform.position);

        return msg;
    }

    public void ValidInteraction(ushort toId, bool valid)
    {
        Message msg = Message.Create(MessageSendMode.Reliable, ServerToClientId.ValidInteract);
        msg.AddBool(valid);
        NetworkManager.Instance.Server.Send(msg, toId);
    }


    public void SendReward(ushort toId, ushort[] reward)
    {
        Message msg = Message.Create(MessageSendMode.Reliable, ServerToClientId.HarvestMsg);
        msg.AddUShorts(reward, false);
        NetworkManager.Instance.Server.Send(msg, toId);
    }


    //-------------------------------------------------------------------------------------//
    //---------------------------- Message Handlers ---------------------------------------//
    //-------------------------------------------------------------------------------------//


}
