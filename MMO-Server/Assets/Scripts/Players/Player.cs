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
    public InventoryData CurrentInventory { get; private set; }
    public GearData CurrentGear { get; private set; }
    public PlayerMovement PlayerMovement { get; private set; }
    public CharacterController Controller { get; private set; }

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
        Controller = GetComponent<CharacterController>();
        Controller.center = Constants.CHARACTER_CONTROLLER_CAPSULE_CENTER;
        transform.position = pos;
        transform.rotation = rot;
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

    public async Task SetInventory(List<float> inventoryData)
    {
        byte size = (byte)inventoryData[0];
        List<Item> items = new List<Item>();
        try
        {
            for(int i = 1; i < inventoryData.Count; i++)
            {
                float item = inventoryData[i];
                ushort id = (ushort)item;
                string itemStr = await ItemManager.GetItemString(id);
                string[] itemDetails = itemStr.Split("`");
                byte slot = byte.Parse(itemDetails[1]);
                Item newItem = new Item(id, itemDetails[0], (Slot)slot);
                items.Add(newItem);
            }
            CurrentInventory = new InventoryData(items, size);
        }
        catch (System.Exception e)
        {
            IDLogger.LogError($"Failed building inventory with Exception: {e}");
        }
    }

    public async Task SetGear(List<float> gearData)
    {
        List<EquippableItem> items = new List<EquippableItem>();
        try
        {
            foreach (var item in gearData)
            {
                ushort id = (ushort)item;
                string itemStr = await ItemManager.GetItemString(id);
                string[] itemDetails = itemStr.Split("`");
                byte slot = byte.Parse(itemDetails[1]);
                bool hasAbility = bool.Parse(itemDetails[2]);
                EquippableItem newItem = new EquippableItem(id, itemDetails[0], (Slot)slot, true, hasAbility);
                items.Add(newItem);
            }
            CurrentGear = new GearData(items);
        }
        catch (System.Exception e)
        {
            IDLogger.LogError($"Failed building gear with Exception: {e}");
        }
    }

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

    public void SendInventory(ushort toId, List<float> inventory)
    {
        Message msg = Message.Create(MessageSendMode.Reliable, (ushort)ServerToClientId.SendInventory);
        msg.AddByte((byte)inventory.Count); //Length of inventory
        msg.AddByte((byte)inventory[0]); //Size of inventory

        //Add each inventory item
        for (int i = 1; i < inventory.Count; i++)
        {
            msg.AddUShort((ushort)inventory[i]);
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


    //-------------------------------------------------------------------------------------//
    //---------------------------- Message Handlers ---------------------------------------//
    //-------------------------------------------------------------------------------------//


}
