using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HairType : byte
{
    Type1,
    Type2,
}

public class HairCustomizer : MonoBehaviour
{
    [SerializeField] private HairType m_Type;
    public HairType Type => m_Type;
}
