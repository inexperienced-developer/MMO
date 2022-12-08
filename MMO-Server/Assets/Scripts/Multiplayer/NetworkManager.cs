using InexperiencedDeveloper.Core;
using Riptide;
using Riptide.Utils;
using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
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
    public Server Server { get; private set; }

    [SerializeField] private ushort m_Port;
    [SerializeField] private ushort m_MaxClientCount;
    public ushort CurrentTick { get; private set; } = 0;

    private void Start()
    {
        RiptideLogger.Initialize(Debug.Log, Debug.Log, Debug.LogWarning, Debug.LogError, true);
        Server = new Server();
        Server.Start(m_Port, m_MaxClientCount);
        Server.ClientDisconnected += PlayerLeft;
    }

    private void FixedUpdate()
    {
        Server.Update();
        if (CurrentTick % 300 == 0)
            SendSync();
        CurrentTick++;
    }

    private void PlayerLeft(object sender, ServerDisconnectedEventArgs e)
    {
        Destroy(PlayerManager.GetPlayerById(e.Client.Id).gameObject);
    }

    private void OnApplicationQuit()
    {
        Server.Stop();
    }

    private void SendSync()
    {
        Message msg = Message.Create(MessageSendMode.Unreliable, (ushort)ServerToClientId.SyncTicks);
        msg.AddUShort(CurrentTick);
        Server.SendToAll(msg);
    }


}
