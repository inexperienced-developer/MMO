using InexperiencedDeveloper.Core;
using InexperiencedDeveloper.MMO.Data;
using InexperiencedDeveloper.Utils;
using InexperiencedDeveloper.Utils.Log;
using Riptide;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class PlayerManager : Singleton<PlayerManager>
{
    private static Dictionary<ushort, Player> m_PlayerList = new();
    private static Dictionary<ushort, InGamePlayer> m_InGamePlayerList = new();
    private static Player m_LocalPlayer;

    //Layer Options
    [SerializeField] protected LayerMask m_NonPlayerLayer;
    public LayerMask NonPlayerLayer => m_NonPlayerLayer;

    public static void RemovePlayerFromList(Player sender)
    {
        m_PlayerList.Remove(sender.Id);
    }

    public static void AddPlayerToList(Player sender)
    {
        m_PlayerList.Add(sender.Id, sender);
    }

    public static Player GetLocalPlayer()
    {
        return m_LocalPlayer;
    }

    private static void Initialize(ushort id, List<CharacterAppearanceData> data, string email)
    {
        if (m_LocalPlayer != null)
        {
            LobbyPlayer localLobbyPlayer = (LobbyPlayer)m_LocalPlayer;
            localLobbyPlayer.SetCharacterList(data);
            localLobbyPlayer.Init(id, email);
            return;
        }
        GameObject go = new GameObject();
        LobbyPlayer player = go.AddComponent<LobbyPlayer>();
        player.name = $"Player {id} ({(string.IsNullOrEmpty(email) ? "Invalid name" : email)})";
        player.SetCharacterList(data);
        player.Init(id, email);
        if (id == NetworkManager.Instance.Client.Id) m_LocalPlayer = player;
    }

    private static void Spawn(ushort id, byte geoArea, Vector3 pos, Quaternion rot, List<ushort> gear, CharacterAppearanceData data)
    {
        LevelManager.Instance.SetLevelToLoad(geoArea);
        InGamePlayer player = null;
        if(id == NetworkManager.Instance.Client.Id)
        {
            CharacterBuilder newChar = CharacterBuilderManager.Instance.CreateNewCharacter(pos, rot);
            GameObject go = newChar.gameObject;
            newChar.SetCharacterAppearance((SkinColor)data.SkinColor, (HairColor)data.HairColor, (HairStyle)data.HairStyle,
                (FacialHairStyle)data.FacialHairStyle, (EyebrowStyle)data.EyebrowStyle, (EyeColor)data.EyeColor, data.BootsOn,
                data.ShirtOn, data.PantsOn);
            player = go.AddComponent<InGamePlayer>();
            m_LocalPlayer = player;
            player.Init(id, GameManager.Email);
            player.name = $"Player {id} ({data.Name}) -- LOCAL PLAYER";
            if (m_InGamePlayerList.ContainsKey(id))
                m_InGamePlayerList[id] = player;
            else
                m_InGamePlayerList.Add(id, player);
            LevelManager.Loaded = true;
        }
        else
        {
            CharacterBuilder newChar = CharacterBuilderManager.Instance.CreateNewCharacter(pos, rot);
            GameObject go = newChar.gameObject;
            newChar.SetCharacterAppearance((SkinColor)data.SkinColor, (HairColor)data.HairColor, (HairStyle)data.HairStyle,
                (FacialHairStyle)data.FacialHairStyle, (EyebrowStyle)data.EyebrowStyle, (EyeColor)data.EyeColor, data.BootsOn,
                data.ShirtOn, data.PantsOn);
            player = go.AddComponent<InGamePlayer>();
            player.Init(id);
            player.name = $"Player {id} ({data.Name})";
            if (m_InGamePlayerList.ContainsKey(id))
                m_InGamePlayerList[id] = player;
            else
                m_InGamePlayerList.Add(id, player);
        }
        IDLogger.Log($"{data.Name} on Player List: {m_InGamePlayerList.ContainsKey(id)}");
    }

    public void MovePlayersToScene(int level)
    {
        foreach (Player p in m_PlayerList.Values)
        {
            LevelManager.Instance.MoveGameObjectToScene(level, p);
        }
    }

    private static void SetInventory(List<ushort> bagIDs, List<ushort> itemIDs)
    {
        GetLocalPlayer().SetInventory(bagIDs, itemIDs);
    }

    private static void SetHarvestType(ushort id, HarvestType harvestType)
    {
        if(!m_PlayerList.TryGetValue(id, out Player player))
        {
            IDLogger.LogError($"Couldn't find player {id} in active player list.");
            return;
        }
        InGamePlayer p = (InGamePlayer)player;
        if(p == null)
        {
            IDLogger.LogError($"Error casting player to InGamePlayer");
            return;
        }
        if(player == GetLocalPlayer())
        {
            if(p.StateMachine.HarvestType != harvestType)
            {
                p.StateMachine.ChangeState(PlayerState.Idle);
                IDLogger.LogError($"HarvestType (client) does not match HarvestType (server).");
                return;
            } 
        }
        else
        {
            p.StateMachine.SetHarvestType(harvestType);
            p.StateMachine.ChangeState(PlayerState.Harvesting);
        }
    }

    private static void SendHarvestReward(ushort[] reward)
    {
        InGamePlayer p = (InGamePlayer)GetLocalPlayer();
        p.ReceiveHarvestReward(reward);
    }

    #region Messages

    //-------------------------------------------------------------------------------------//
    //---------------------------- Message Sending ----------------------------------------//
    //-------------------------------------------------------------------------------------//

    public void SendCreatedNewCharacter(CharacterAppearanceData data)
    {
        Message msg = Message.Create(MessageSendMode.Reliable, (ushort)ClientToServerId.CreateNewCharacter);
        IDLogger.Log("Sending new character request");
        msg = AddCharacterAppearanceMessage(msg, data);
        NetworkManager.Instance.Client.Send(msg);
    }
    private Message AddCharacterAppearanceMessage(Message msg, CharacterAppearanceData data)
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
        byte appearanceByte = Utilities.BoolsToByte(new bool[3] { data.PantsOn, data.ShirtOn, data.BootsOn });
        msg.AddByte(appearanceByte);
        return msg;
    }
    
    public void DeleteCharacter()
    {
        LobbyPlayer lobbyPlayer = (LobbyPlayer)m_LocalPlayer;
        Message msg = Message.Create(MessageSendMode.Reliable, (ushort)ClientToServerId.DeleteCharacter);
        string charName = lobbyPlayer.SelectedCharacter.Name;
        string email = lobbyPlayer.Email;
        msg.AddString(email);
        msg.AddString(charName);
        NetworkManager.Instance.Client.Send(msg);
        lobbyPlayer.DeleteCharacter();
    }

    public void RequestSpawnPlayer(string email, string characterName)
    {
        Message msg = Message.Create(MessageSendMode.Reliable, (ushort)ClientToServerId.RequestSpawn);
        msg.AddString(email);
        msg.AddString(characterName);
        NetworkManager.Instance.Client.Send(msg);
    }

    //-------------------------------------------------------------------------------------//
    //---------------------------- Message Handlers ---------------------------------------//
    //-------------------------------------------------------------------------------------//
    [MessageHandler((ushort)ServerToClientId.AccountCharacterData)]
    private static void ReceiveCharacterData(Message msg)
    {
        IDLogger.Log("Receiving Data");
        ushort id = msg.GetUShort();
        string email = msg.GetString();
        byte length = msg.GetByte();
        IDLogger.Log($"Length: {length}");
        List<CharacterAppearanceData> data = ReceiveDataList(msg, length);
        Initialize(id, data, email);    
    }

    private static List<CharacterAppearanceData> ReceiveDataList(Message msg, byte length = 1)
    {
        List<CharacterAppearanceData> data = new List<CharacterAppearanceData>();
        for (byte i = 0; i < length; i++)
        {
            string name = msg.GetString();
            byte level = msg.GetByte();
            ushort totalLevel = msg.GetUShort();
            byte skinColor = msg.GetByte();
            byte hairColor = msg.GetByte();
            byte hairStyle = msg.GetByte();
            byte facialHairStyle = msg.GetByte();
            byte eyebrowStyle = msg.GetByte();
            byte eyeColor = msg.GetByte();
            byte appearanceByte = msg.GetByte();
            bool[] appearanceBools = Utilities.ByteToBools(appearanceByte, 3);
            CharacterAppearanceData charData = new CharacterAppearanceData(name, level, totalLevel,
                skinColor, hairColor, hairStyle, facialHairStyle, eyebrowStyle, eyeColor,
                appearanceBools[2], appearanceBools[1], appearanceBools[0]);
            data.Add(charData);
        }
        return data;
    }
    
    [MessageHandler((ushort)ServerToClientId.SpawnPlayer)]
    private static void SpawnPlayer(Message msg)
    {
        ushort id = msg.GetUShort();
        byte geoArea = msg.GetByte();
        Vector3 pos = msg.GetVector3Int();
        Quaternion rot = msg.GetQuaternionInt();
        byte gearLength = msg.GetByte();
        List<ushort> gear = new(); 
        for(int i = 0; i < gearLength; i++)
        {
            gear.Add(msg.GetUShort());
        }
        CharacterAppearanceData data = ReceiveDataList(msg)[0];
        Spawn(id, geoArea, pos, rot, gear, data);
    }

    [MessageHandler((ushort)ServerToClientId.SendInventory)]
    private static void ReceiveInventory(Message msg)
    {
        byte bagLength = msg.GetByte();
        List<ushort> bagIDs = new();
        for (int i = 0; i < bagLength; i++)
        {
            bagIDs.Add(msg.GetUShort());
        }
        byte inventoryLength = msg.GetByte();
        List<ushort> inventoryIDs = new();
        for(int i = 0; i < inventoryLength; i++)
        {
            inventoryIDs.Add(msg.GetUShort());
        }
        SetInventory(bagIDs, inventoryIDs);
    }

    [MessageHandler((ushort)ServerToClientId.UpdatePosition)]
    //Receives 12 Bytes per call per player
    private static void ReceiveMovement(Message msg)
    {
        ushort id = msg.GetUShort(); // 1Byte
        Vector3 pos = msg.GetVector3Int(); // 8 Bytes
        byte inputByte = msg.GetByte(); // 1 Byte
        bool[] input = Utilities.ByteToBools(inputByte, 7);
        float x = input[0] ? 1 : 0;
        x = input[1] ? -1 : x;
        float y = input[2] ? 1 : 0;
        y = input[3] ? -1 : y;
        if (x > 0 && y > 0)
        {
            x = 0.5f;
            y = 0.5f;
        }
        Vector2 move = new Vector2(x, y);
        bool jump = input[4];
        bool rightClick = input[5];
        bool leftClick = input[6];
        float yRot = msg.GetFloatInt(); // 2 Bytes
        if (m_InGamePlayerList.TryGetValue(id, out InGamePlayer p))
        {
            p.ReceiveMovement(pos, yRot, input);
        }
    }

    [MessageHandler((ushort)ServerToClientId.HandleHarvestType)]
    private static void ReceiveHarvestType(Message msg)
    {
        ushort id = msg.GetUShort();
        HarvestType harvestType = (HarvestType)msg.GetByte();
        SetHarvestType(id, harvestType);
    }

    [MessageHandler((ushort)ServerToClientId.HarvestMsg)]
    private static void ReceiveHarvestMessage(Message msg)
    {
        ushort[] reward = msg.GetUShorts(2);
        SendHarvestReward(reward);
    }

    #endregion
}
