using UnityEngine;

[CreateAssetMenu(fileName = "Character/Appearance")]
public class CharacterAppearanceSettings : ScriptableObject
{
    [Header("Skin/Body")]
    public Material[] BodyMats;
    public Material[] ShirtMats;
    public Material[] PantsMats;
    public Material[] HeadMats;
    public Material[] EyeMats;

    [Header("Hair")]
    public GameObject[] HairStyles;
    public Material[] HairMatsType1;
    public Material[] HairMatsType2;

    [Header("Facial Hair")]
    public GameObject[] EyebrowStyles;
    public GameObject[] BeardStyles;
    public Material[] EyebrowsAndBeardMats;
}
