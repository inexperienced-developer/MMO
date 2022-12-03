using InexperiencedDeveloper.Core;
using InexperiencedDeveloper.MMO.Data;
using InexperiencedDeveloper.Utils.Log;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBuilderManager : Singleton<CharacterBuilderManager>
{
    [Header("Create New Character")]
    [SerializeField] private GameObject m_NewCharacter;
    [SerializeField] private Transform m_SpawnPoint;

    public CharacterBuilder CreateNewCharacter()
    {
        GameObject go = Instantiate(m_NewCharacter, m_SpawnPoint);
        return go.GetComponent<CharacterBuilder>();
    }

    public CharacterBuilder CreateNewCharacter(Vector3 pos, Quaternion rot)
    {
        GameObject go = Instantiate(m_NewCharacter, pos, rot);
        return go.GetComponent<CharacterBuilder>();
    }
}
