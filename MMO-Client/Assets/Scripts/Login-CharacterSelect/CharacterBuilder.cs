using InexperiencedDeveloper.Utils.Log;
using System.Linq;
using UnityEngine;


public class CharacterBuilder : MonoBehaviour
{
    public string Name;
    [Header("Appearance Settings")]
    [SerializeField] private CharacterAppearanceSettings m_AppearanceSettings;
    [SerializeField] private SkinColor m_CurrentSkinColor = SkinColor.WhiteMed;
    [SerializeField] private ShirtColor m_CurrentShirtColor = ShirtColor.Blue;
    [SerializeField] private PantsColor m_CurrentPantsColor = PantsColor.Blue;
    [SerializeField] private HairColor m_CurrentHairColor = HairColor.Brown;
    [SerializeField] private HairStyle m_CurrentHairStyle = HairStyle.Messy;
    [SerializeField] private FacialHairStyle m_CurrentFacialHairStyle = FacialHairStyle.None;
    [SerializeField] private EyebrowStyle m_CurrentEyebrowStyle = EyebrowStyle.Regular;
    [SerializeField] private EyeColor m_CurrentEyeColor = EyeColor.Blue;
    public CharacterAppearanceSettings AppearanceSettings => m_AppearanceSettings;
    public SkinColor CurrentSkinColor
    {
        get
        {
            return m_CurrentSkinColor;
        }
        set
        {
            m_CurrentSkinColor = value;
            ChangeMat();
        }
    }
    public HairColor CurrentHairColor
    {
        get
        {
            return m_CurrentHairColor;
        }
        set
        {
            m_CurrentHairColor = value;
            ChangeHairColor();
        }
    }
    public HairStyle CurrentHairStyle
    {
        get
        {
            return m_CurrentHairStyle;
        }
        set
        {
            m_CurrentHairStyle = value;
            ChangeHairStyle();
        }
    }

    public FacialHairStyle CurrentFacialHairStyle
    {
        get
        {
            return m_CurrentFacialHairStyle;
        }
        set
        {
            m_CurrentFacialHairStyle = value;
            ChangeFacialHairStyle();
        }
    }

    public EyebrowStyle CurrentEyebrowStyle
    {
        get
        {
            return m_CurrentEyebrowStyle;
        }
        set
        {
            m_CurrentEyebrowStyle = value;
            ChangeEyebrowStyle();
        }
    }

    public EyeColor CurrentEyeColor
    {
        get
        {
            return m_CurrentEyeColor;
        }
        set
        {
            m_CurrentEyeColor = value;
            ChangeEyeColor();
        }
    }

    [Header("Renderers Settings")]
    [SerializeField] private SkinnedMeshRenderer[] m_BodyRenderers;
    [SerializeField] private SkinnedMeshRenderer[] m_PantsRenderers;
    [SerializeField] private SkinnedMeshRenderer[] m_SkinRenderers;
    [SerializeField] private SkinnedMeshRenderer[] m_HeadRenderers;
    [SerializeField] private Renderer m_EyeRenderer;

    [Header("Equipment Settings")]
    [SerializeField] private Transform m_HairPoint;
    [SerializeField] private GameObject m_CurrentHairObj;
    [SerializeField] private Transform m_FacialHairPoint;
    [SerializeField] private GameObject m_CurrentFacialHairObj;
    [SerializeField] private GameObject m_CurrentEyebrowObj;
    [SerializeField] private GameObject[] m_Boots;
    [SerializeField] private GameObject m_Feet;

    [Header("Equip/Unequip Options")]
    [SerializeField] private bool m_BootsOn;
    [SerializeField] private bool m_ShirtOn;
    [SerializeField] private bool m_PantsOn;
    public bool[] AppearanceBools
    {
        get
        {
            return new bool[3] { m_PantsOn, m_ShirtOn, m_BootsOn };
        }
    }

    private void Awake()
    {
    }

    public void NewRandomCharacter()
    {
        CurrentSkinColor = (SkinColor)Random.Range(0, (int)System.Enum.GetValues(typeof(SkinColor)).Cast<SkinColor>().Max() + 1);
        CurrentHairColor = (HairColor)Random.Range(0, (int)System.Enum.GetValues(typeof(HairColor)).Cast<HairColor>().Max() + 1);
        CurrentHairStyle = (HairStyle)Random.Range(0, (int)System.Enum.GetValues(typeof(HairStyle)).Cast<HairStyle>().Max() + 1);
        CurrentFacialHairStyle = (FacialHairStyle)Random.Range(0, (int)System.Enum.GetValues(typeof(FacialHairStyle)).Cast<FacialHairStyle>().Max());
        CurrentEyebrowStyle = (EyebrowStyle)Random.Range(0, (int)System.Enum.GetValues(typeof(EyebrowStyle)).Cast<EyebrowStyle>().Max() + 1);
        CurrentEyeColor = (EyeColor)Random.Range(0, (int)System.Enum.GetValues(typeof(EyeColor)).Cast<EyeColor>().Max() + 1);
    }

    public void SetCharacterAppearance(SkinColor skinColor, HairColor hairColor, HairStyle hairStyle, FacialHairStyle facialHairStyle, EyebrowStyle eyebrowStyle, EyeColor eyeColor, bool bootsOn, bool shirtOn, bool pantsOn)
    {
        m_BootsOn = bootsOn;
        m_ShirtOn = shirtOn;
        m_PantsOn = pantsOn;
        CurrentSkinColor = skinColor;
        CurrentHairColor = hairColor;
        CurrentHairStyle = hairStyle;
        CurrentFacialHairStyle = facialHairStyle;
        CurrentEyebrowStyle = eyebrowStyle;
        CurrentEyeColor = eyeColor;
        Destroy(this);
    }

    [ContextMenu("Change Mat")]
    public void ChangeMat()
    {
        SetSkinColor();
        SetShirtOn(m_ShirtOn);
        SetPantsOn(m_PantsOn);
        SetBootsOn(m_BootsOn);
    }

    public void ChangeHairStyle()
    {
        if(m_CurrentHairObj != null)
            Destroy(m_CurrentHairObj);
        if (m_CurrentHairStyle == HairStyle.Bald) return;
        m_CurrentHairObj = Instantiate(m_AppearanceSettings.HairStyles[(int)m_CurrentHairStyle], m_HairPoint);
        ChangeHairColor();
    }

    public void ChangeFacialHairStyle()
    {
        if (m_CurrentFacialHairObj != null)
            Destroy(m_CurrentFacialHairObj);
        if (m_CurrentFacialHairStyle == FacialHairStyle.None) return;
        m_CurrentFacialHairObj = Instantiate(m_AppearanceSettings.BeardStyles[(int)m_CurrentFacialHairStyle], m_FacialHairPoint);
        ChangeHairColor();
    }

    public void ChangeEyebrowStyle()
    {
        if (m_CurrentEyebrowObj != null)
            Destroy(m_CurrentEyebrowObj);
        m_CurrentEyebrowObj = Instantiate(m_AppearanceSettings.EyebrowStyles[(int)m_CurrentEyebrowStyle], m_FacialHairPoint);
        ChangeHairColor();
    }

    public void ChangeHairColor()
    {
        ChangeFacialHairColor();
        if (m_CurrentHairObj == null) return;
        HairCustomizer hair = m_CurrentHairObj.GetComponent<HairCustomizer>();
        Material[] hairMats;
        switch (hair.Type)
        {
            case HairType.Type1:
            default:
                hairMats = m_AppearanceSettings.HairMatsType1;
                break;
            case HairType.Type2:
                hairMats = m_AppearanceSettings.HairMatsType2;
                break;
        }
        Material hairMat = hairMats[(int)m_CurrentHairColor];
        foreach(var r in m_CurrentHairObj.GetComponentsInChildren<Renderer>())
            r.material = hairMat;
    }

    private void ChangeFacialHairColor()
    {
        Material facialMat = m_AppearanceSettings.EyebrowsAndBeardMats[(int)m_CurrentHairColor];
        if(m_CurrentFacialHairObj != null)
        {
            foreach(var r in m_CurrentFacialHairObj.GetComponentsInChildren<Renderer>())
                r.material = facialMat;
        }
        if (m_CurrentEyebrowObj != null)
        {
            foreach (var r in m_CurrentEyebrowObj.GetComponentsInChildren<Renderer>())
                r.material = facialMat;
        }
    }

    public void ChangeEyeColor()
    {
        Material eyeMat = m_AppearanceSettings.EyeMats[(int)m_CurrentEyeColor];
        m_EyeRenderer.material = eyeMat;
    }

    private void SetSkinColor()
    {
        Material bodyMat = m_AppearanceSettings.BodyMats[(int)m_CurrentSkinColor];
        Material headMat = m_AppearanceSettings.HeadMats[(int)m_CurrentSkinColor];
        //Skin
        foreach (var r in m_SkinRenderers)
        {
            r.material = bodyMat;
        }
        //Head
        foreach (var r in m_HeadRenderers)
        {
            r.material = headMat;
        }
    }

    private void SetShirtOn(bool shirtOn)
    {
        Material bodyMat = m_AppearanceSettings.BodyMats[(int)m_CurrentSkinColor];
        Material shirtMat = m_AppearanceSettings.ShirtMats[(int)m_CurrentShirtColor];
        if (!shirtOn)
        {
            shirtMat = bodyMat;
        }
        foreach (var r in m_BodyRenderers)
        {
            Material[] mats = r.sharedMaterials;
            mats[0] = bodyMat;
            mats[1] = shirtMat;
            r.sharedMaterials = mats;
        }
    }

    private void SetPantsOn(bool pantsOn)
    {
        Material bodyMat = m_AppearanceSettings.BodyMats[(int)m_CurrentSkinColor];
        Material pantsMat = m_AppearanceSettings.PantsMats[(int)m_CurrentPantsColor];
        if (pantsOn)
        {
            bodyMat = pantsMat;
        }
        foreach (var r in m_PantsRenderers)
        {
            r.sharedMaterial = r.gameObject.name.Contains("Boots") ? bodyMat : pantsMat;
        }
    }

    private void SetBootsOn(bool bootsOn)
    {
        foreach (var b in m_Boots)
            b.SetActive(bootsOn);
        m_Feet.SetActive(!bootsOn);
    }
}
