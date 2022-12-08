using InexperiencedDeveloper.Core;
using Riptide;
using Riptide.Utils;
using System;
using UnityEngine;

public enum ServerToClientId : ushort
{
    SyncTicks = 1,
    AccountCharacterData,
    ReceiveCharacterExists,
    SpawnPlayer,
    SendInventory,
    UpdatePosition,
    ValidInteract,
    HandleHarvestType,
    HarvestMsg,
    SendState,
}

public enum ClientToServerId : ushort
{
    AccountInformation = 1,
    CreateNewCharacter,
    CheckCharacterExists,
    DeleteCharacter,
    RequestSpawn,
    MoveRequest,
    RequestInteract,
    SendHarvestType,
    ReceiveState,
}

public class NetworkManager : Singleton<NetworkManager>
{
    public Client Client { get; private set; }
    [SerializeField] private string m_Ip;
    [SerializeField] private ushort m_Port;

    private void Start()
    {
        RiptideLogger.Initialize(Debug.Log, Debug.Log, Debug.LogWarning, Debug.LogError, true);
        Client = new Client();
        Client.Connected += DidConnect;
        Client.ConnectionFailed += FailedToConnect;
        Client.Disconnected += DidDisconnect;
    }

    private void FixedUpdate()
    {
        Client.Update();
    }

    public void Connect()
    {
        Client.Connect($"{m_Ip}:{m_Port}");
    }

    public void Disconnect()
    {
        Client.Disconnect();
    }

    private void DidConnect(object sender, EventArgs e)
    {
        LobbyUIManager.Instance.SendName();
    }

    private void FailedToConnect(object sender, EventArgs e)
    {
        LobbyUIManager.Instance.BackToLogin();
    }

    private void DidDisconnect(object sender, EventArgs e)
    {
        LobbyUIManager.Instance.BackToLogin();
    }

    private void OnApplicationQuit()
    {
        Client.Disconnect();
    }
}
