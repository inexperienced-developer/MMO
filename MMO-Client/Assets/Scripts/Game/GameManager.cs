using InexperiencedDeveloper.Core;
using InexperiencedDeveloper.MMO.Data;
using InexperiencedDeveloper.Utils;
using InexperiencedDeveloper.Utils.Log;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    [Header("Prefabs")]
    [SerializeField] private Transform m_SpawnPoint;
    public Transform SpawnPoint
    {
        get
        {
            m_SpawnPoint = m_SpawnPoint == null ? GameObject.Find("SpawnPoint").transform : m_SpawnPoint;
            return m_SpawnPoint;
        }
    }
    [SerializeField] private GameObject m_CharacterPrefab;
    [SerializeField] private GameObject m_LocalCameraPrefab;
    public GameObject LocalCameraPrefab => m_LocalCameraPrefab;

    //[Header("Character")]
    public static GameObject SelectedCharacter { get; private set; }
    public static string Email { get; private set; }
    public static string CharacterName { get; private set; }
    public const byte NUM_OF_SKILLS = 9;
    

    protected override void Awake()
    {
        base.Awake();
        Cursor.lockState = CursorLockMode.Confined;
    }

    public GameObject SpawnCharacter(string name, byte level, ushort totalLevel,
        byte skinColor, byte hairColor, byte hairStyle, byte facialHairStyle, byte eyebrowStyle,
        byte eyeColor, bool bootsOn, bool shirtOn, bool pantsOn)
    {
        GameObject go = Instantiate(m_CharacterPrefab, m_SpawnPoint);
        CharacterBuilder builder = go.GetComponent<CharacterBuilder>();
        go.name = $"{name}";
        builder.SetCharacterAppearance((SkinColor)skinColor, (HairColor)hairColor, (HairStyle)hairStyle, (FacialHairStyle)facialHairStyle,
            (EyebrowStyle)eyebrowStyle, (EyeColor)eyeColor, bootsOn, shirtOn, pantsOn);
        return go;
    }

    public GameObject SpawnCharacter(CharacterAppearanceData data)
    {
        GameObject go = Instantiate(m_CharacterPrefab, m_SpawnPoint);
        CharacterBuilder builder = go.GetComponent<CharacterBuilder>();
        go.name = $"{data.Name}";
        builder.SetCharacterAppearance((SkinColor)data.SkinColor, (HairColor)data.HairColor, (HairStyle)data.HairStyle, (FacialHairStyle)data.FacialHairStyle,
            (EyebrowStyle)data.EyebrowStyle, (EyeColor)data.EyeColor, data.BootsOn, data.ShirtOn, data.PantsOn);
        return go;
    }

    public void LoadLevelPersistCharacter(GameObject character, string email, string characterName)
    {
        SelectedCharacter = character;
        Email = email;
        CharacterName = characterName;
        IDLogger.Log($"Set Character Name: {CharacterName}, Email: {Email}, and Character");
        //Load Level Loading
        LevelManager.Instance.LoadLevel();
    }
}
