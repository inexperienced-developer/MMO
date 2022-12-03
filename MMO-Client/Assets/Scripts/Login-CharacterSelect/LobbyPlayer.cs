using InexperiencedDeveloper.MMO.Data;
using Riptide;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LobbyPlayer : Player
{
    public List<CharacterAppearanceData> Characters = new List<CharacterAppearanceData>();
    public CharacterAppearanceData SelectedCharacter;
    private GameObject m_SelectedCharacterGO;
    public GameObject SelectedCharacterGO
    {
        get
        {
            return m_SelectedCharacterGO;
        }
        set
        {
            m_SelectedCharacterGO = value;
            LobbyUIManager.Instance.SetCharacterNameText(SelectedCharacter.Name);
        }
    }
    public Dictionary<string, GameObject> SpawnedCharacters = new Dictionary<string, GameObject>();

    public void SetCharacterList(List<CharacterAppearanceData> data)
    {
        Characters = data;
    }

    public override void Init(ushort id, string email)
    {
        base.Init(id, email);
        if (Id == NetworkManager.Instance.Client.Id)
        {
            List<CharacterAppearanceData> orderedByLevel = Characters.OrderBy(o => o.Level).ToList();
            LobbyUIManager.Instance.PopulateCharacters(Characters);
            foreach (Transform k in GameManager.Instance.SpawnPoint)
                Destroy(k.gameObject);
            SpawnedCharacters.Clear();
            for(int i = 0; i < Characters.Count; i++)
            {
                GameObject character = GameManager.Instance.SpawnCharacter(Characters[i].Name, Characters[i].Level,
                    Characters[i].TotalLevel, Characters[i].SkinColor, Characters[i].HairColor,
                    Characters[i].HairStyle, Characters[i].FacialHairStyle, Characters[i].EyebrowStyle,
                    Characters[i].EyeColor, Characters[i].BootsOn, Characters[i].ShirtOn, Characters[i].PantsOn);
                SpawnedCharacters.Add(Characters[i].Name, character);
                character.SetActive(false);
            }
            if(Characters.Count > 0)
            {
                SelectedCharacter = Characters[0];
                SelectedCharacterGO = SpawnedCharacters[Characters[0].Name];
                SelectedCharacterGO.SetActive(true);
            }
        }
    }

    public void AddSpawnedCharacter(CharacterAppearanceData data)
    {
        //Add spawned character
        GameObject character = GameManager.Instance.SpawnCharacter(data);
        SpawnedCharacters.Add(data.Name, character);
        SelectedCharacter = data;
        SelectedCharacterGO = character;
        //Send create request to server
    }

    public void DeleteCharacter()
    {
        for(int i = 0; i < Characters.Count; i++)
        {
            if (Characters[i].Name == SelectedCharacter.Name)
            {
                Characters.Remove(Characters[i]);
                SpawnedCharacters.Remove(SelectedCharacter.Name);
                if(Characters.Count > 0)
                    SelectedCharacter = Characters[i];
            }
        }
        Destroy(SelectedCharacterGO);
        if (Characters.Count > 0)
            SelectedCharacterGO = SpawnedCharacters[SelectedCharacter.Name];
        else
        {
            SelectedCharacterGO = null;
            LobbyUIManager.Instance.SetCharacterNameText("");
        }
    }
}
