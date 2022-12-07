using InexperiencedDeveloper.Core;
using InexperiencedDeveloper.Firebase;
using InexperiencedDeveloper.MMO.Data;
using InexperiencedDeveloper.Utils;
using InexperiencedDeveloper.Utils.Log;
using Riptide;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerManager : Singleton<PlayerManager>
{
    private static Dictionary<ushort, Player> m_PlayerList = new();

    public static Player GetPlayerById(ushort id)
    {
        if (m_PlayerList.TryGetValue(id, out Player player))
            return player;
        else
        {
            IDLogger.LogError("Player does not exist");
            return null;
        }
    }

    public static void RemovePlayerFromList(Player sender)
    {
        m_PlayerList.Remove(sender.Id);
    }

    public List<Player> GetNearbyPlayers(Vector3 pos)
    {
        List<Player> players = new();
        foreach(Player p in m_PlayerList.Values)
        {
            if(Vector3.Distance(p.transform.position, pos) <= ConstGeoAreas.MAX_SPAWN_DISTANCE)
            {
                players.Add(p);
            }
        }
        return players;
    }

    //private static void Spawn(ushort id, string username)
    //{
    //    foreach(var p in PlayerList.Values)
    //    {
    //        p.SendSpawned(id);
    //    }

    //    Player player = Instantiate(GameManager.Instance.PlayerPrefab, Vector3.up, Quaternion.identity).GetComponent<Player>();
    //    player.name = $"Player {id} ({(string.IsNullOrEmpty(username) ? "Invalid name" : username)})";
    //    player.Init(id, username);
    //    //Send spawned info to all players
    //    player.SendSpawned();
    //    PlayerList.Add(id, player);
    //}

    private async static Task BuildProfile(ushort id, string email)
    {
        try
        {
            List<CharacterAppearanceData> res = await DBManager.Instance.GetCharacters(email);
            FillData(id, res, email);
        } catch (System.Exception e)
        {
            IDLogger.LogError($"Exception: {e}");
        }
    }

    private static void FillData(ushort id, List<CharacterAppearanceData> data, string email)
    {
        GameObject go = Instantiate(GameManager.Instance.PlayerPrefab, Vector3.zero, Quaternion.identity);
        Player player = go.GetComponent<Player>() == null ? go.AddComponent<Player>() : go.GetComponent<Player>();
        player.name = $"Player {id} ({(string.IsNullOrEmpty(email) ? "Invalid name" : email)})";
        player.Init(id, email, data);
        player.SendCharacterList(data);
        m_PlayerList.Add(id, player);
    }

    private async static Task CheckCharacterExistsInDB(ushort id, string n)
    {
        try
        {
            bool characterExists = await DBManager.Instance.CheckCharacterExists(n);
            SendCharacterExists(id, characterExists);
        } catch (System.Exception e)
        {
            IDLogger.LogError($"Exception: {e}");
        }
    }

    private async static Task AddNewCharacterToDB(ushort id, CharacterAppearanceData data)
    {
        m_PlayerList.TryGetValue(id, out Player player);
        if(player == null) { IDLogger.LogError($"Can't find player {id}"); return; }
        try
        {
            List<CharacterAppearanceData> res = await DBManager.Instance.AddNewCharToDB(player.Email, data);
            player.SendCharacterList(res);
            player.Init(id, player.Email, res);
        }
        catch (System.Exception e)
        {
            IDLogger.LogError($"Exception: {e}");
        }
    }

    private async static Task DeleteCharacterFromDB(ushort id, string email, string n)
    {
        m_PlayerList.TryGetValue(id, out Player player);
        try
        {
            List<CharacterAppearanceData> res = await DBManager.Instance.DeleteCharFromDB(email, n);
            if(res != null)
            {
                player.Init(id, player.Email, res);
                player.SendCharacterList(res);
            }
        }
        catch (System.Exception e)
        {
            IDLogger.LogError($"Exception: {e}");
        }
    }

    private async static Task GenerateSpawnDataFromDB(ushort id, string email, string n)
    {
        m_PlayerList.TryGetValue(id, out Player player);
        try
        {
            player.SetSpawnedCharacter(n);
            //Get Inventory, Gear and Position of Player
            List<List<float>> res = await DBManager.Instance.GetSpawnDataFromDB(email, n);

            //Separate out DB result
            Vector3 pos = new Vector3(res[0][0], res[0][1], res[0][2]);
            byte geoArea = (byte)res[0][3];
            Quaternion rot = new Quaternion(res[1][0], res[1][1], res[1][2], res[1][3]);
            List<float> gear = res[2];
            List<ushort> bagIDs = res[3].Select(x => (ushort)x).ToList();
            List<ushort> inventoryIDs = res[4].Select(x => (ushort)x).ToList();

            foreach (var p in m_PlayerList.Values)
            {
                try
                {
                    if (p.Id == id) continue;
                    //Send other player info to requestor
                    List<List<float>> r = await DBManager.Instance.GetSpawnDataFromDB(p.Email, p.SpawnedCharacter.Name);
                    if (r == null) { IDLogger.LogWarning($"Could not load Spawn data"); continue; }
                    //Separate DB result
                    Vector3 otherPos = new Vector3(r[0][0], r[0][1], r[0][2]);
                    byte otherGeoArea = (byte)r[0][3];
                    Quaternion otherRot = new Quaternion(r[1][0], r[1][1], r[1][2], r[1][3]);
                    List<float> otherGear = r[2];

                    if(Vector3.Distance(otherPos, pos) <= ConstGeoAreas.MAX_SPAWN_DISTANCE)
                    {
                        //Send to requestor
                        p.SendSpawned(id, otherPos, otherRot, otherGear, otherGeoArea, p.Id);
                    }
                }
                catch (System.Exception e)
                {
                    IDLogger.LogError($"Exception: {e}");
                }
            }

            InventoryData inventory = new InventoryData(bagIDs, inventoryIDs);

            //Send spawn data to everyone
            player.SendSpawned(pos, rot, gear, geoArea, id);

            //Send inventory data just to requesting player
            player.SendInventory(id, inventory);

            //Set player data on Server
            player.InitSpawn(pos, rot);
            player.SetInventoryData(inventory);
            //await player.SetGear(gear);
        }
        catch (System.Exception e)
        {
            IDLogger.LogError($"Exception: {e}");
        }
    }

    private static void ValidateInteractRequest(ushort playerId, Vector3 targetPos)
    {
        if(m_PlayerList.TryGetValue(playerId, out Player player))
        {
            Vector3 playerForward = player.transform.forward;
            Vector3 dir = targetPos - player.transform.position;
            float dot = Vector3.Dot(playerForward, dir.normalized);
            IDLogger.Log($"Dot from player to Object {dot}");
            bool valid = Vector3.Distance(player.transform.position, targetPos) < Constants.PLAYER_INTERACT_DISTANCE && dot > 0;
            if (valid)
            {
                Collider[] hits = Physics.OverlapSphere(targetPos, 1);
                foreach(var hit in hits)
                {
                    if (hit.GetComponent<IInteractable>() != null)
                    {
                        InteractType type = hit.GetComponent<IInteractable>().GetInteractType();
                    }
                }
            }
            player.ValidInteraction(player.Id, valid);
            if (valid)
            {
                dir.y = 0;
                Quaternion rot = Quaternion.LookRotation(dir);
                player.transform.rotation = rot;
            }
        }
        else
        {
            IDLogger.LogError($"Could not find player with Player ID: {playerId}");
        }
    }

    #region Messages

    //-------------------------------------------------------------------------------------//
    //---------------------------- Message Sending ----------------------------------------//
    //-------------------------------------------------------------------------------------//

    private static void SendCharacterExists(ushort toId, bool val)
    {
        IDLogger.Log($"Character exists: {val}");
        Message msg = Message.Create(MessageSendMode.Reliable, (ushort)ServerToClientId.ReceiveCharacterExists);
        msg.AddBool(val);
        NetworkManager.Instance.Server.Send(msg, toId);
    }

    //-------------------------------------------------------------------------------------//
    //---------------------------- Message Handlers ---------------------------------------//
    //-------------------------------------------------------------------------------------//
    [MessageHandler((ushort)ClientToServerId.AccountInformation)]
    private async static void ReceiveAccountInfo(ushort fromClientId, Message msg)
    {
        var email = msg.GetString();
        await BuildProfile(fromClientId, email);
    }

    [MessageHandler((ushort)ClientToServerId.CheckCharacterExists)]
    private async static void CheckCharacterExists(ushort fromClientId, Message msg)
    {
        IDLogger.Log($"Checking Character Exists");
        string n = msg.GetString();
        IDLogger.Log("Received string " + n);
        await CheckCharacterExistsInDB(fromClientId, n);
    }

    [MessageHandler((ushort)ClientToServerId.CreateNewCharacter)]
    private async static void ReceiveNewCreatedCharacter(ushort fromClientId, Message msg)
    {
        IDLogger.Log("Receiving New Character");
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
        CharacterAppearanceData data = new CharacterAppearanceData(name, level, totalLevel, skinColor, hairColor, hairStyle, facialHairStyle, eyebrowStyle, eyeColor, appearanceByte);
        await AddNewCharacterToDB(fromClientId, data);
    }

    [MessageHandler((ushort)ClientToServerId.DeleteCharacter)]
    private async static void DeleteCharacter(ushort fromClientId, Message msg)
    {
        IDLogger.Log("Receiving Delete Character");
        string email = msg.GetString();
        string name = msg.GetString();
        await DeleteCharacterFromDB(fromClientId, email, name);
    }

    [MessageHandler((ushort)ClientToServerId.RequestSpawn)]
    private async static void ReceiveSpawnRequest(ushort fromClientId, Message msg)
    {
        IDLogger.Log("Receiving Spawn Request");
        string email = msg.GetString();
        string characterName = msg.GetString();
        await GenerateSpawnDataFromDB(fromClientId, email, characterName);
    }

    [MessageHandler((ushort)ClientToServerId.MoveRequest)]
    private static void ReceiveInputs(ushort fromClientId, Message msg)
    {
        byte inputByte = msg.GetByte();
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
        float look = msg.GetFloatInt();
        if (m_PlayerList.TryGetValue(fromClientId, out Player player))
            player.PlayerMovement.SetInput(move, look, jump, rightClick, leftClick);
    }

    [MessageHandler((ushort)ClientToServerId.RequestInteract)]
    private static void ReceiveInteractRequest(ushort fromClientId, Message msg)
    {
        Vector3 targetPos = msg.GetVector3Int();
        ValidateInteractRequest(fromClientId, targetPos);
    }
    #endregion
}
