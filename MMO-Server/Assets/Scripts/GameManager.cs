using InexperiencedDeveloper.Core;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public const int DEFAULT_INVENTORY_SIZE = 16;

    [Header("Prefabs")]
    [SerializeField] private GameObject m_PlayerPrefab;
    public GameObject PlayerPrefab => m_PlayerPrefab;

    [SerializeField] private Transform m_SpawnPoint;
    public Transform SpawnPoint => m_SpawnPoint;

}
