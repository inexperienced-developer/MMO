using InexperiencedDeveloper.Core;
using InexperiencedDeveloper.Utils.Log;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public const int DEFAULT_INVENTORY_SIZE = 16;

    [Header("Prefabs")]
    [SerializeField] private GameObject m_PlayerPrefab;
    public GameObject PlayerPrefab => m_PlayerPrefab;

    [SerializeField] private Transform m_SpawnPoint;
    public Transform SpawnPoint => m_SpawnPoint;


    [ContextMenu("Check length Item Dict")]
    public void CheckLen()
    {
        IDLogger.Log(ItemManager.Items.Keys.Count.ToString());
    }

    [ContextMenu("Test Dict")]
    public void Test()
    {
        string val = ItemManager.Items["000000"].ItemName;
        IDLogger.Log($"{val}");
    }

}
