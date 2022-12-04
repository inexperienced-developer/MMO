using Firebase;
using Firebase.Auth;
using InexperiencedDeveloper.Core;
using InexperiencedDeveloper.MMO.Data;
using InexperiencedDeveloper.Utils;
using InexperiencedDeveloper.Utils.Log;
using Riptide;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUIManager : Singleton<LobbyUIManager>
{
    [Header("Login")]
    [SerializeField] private GameObject m_Login;
    [SerializeField] private TMP_InputField m_Email;
    [SerializeField] private TMP_InputField m_Password;
    [SerializeField] private GameObject m_ConnectingPanel;

    [Header("Register")]
    [SerializeField] private GameObject m_Register;
    [SerializeField] private TMP_InputField m_EmailReg;
    [SerializeField] private TMP_InputField[] m_PasswordReg = new TMP_InputField[2];

    [Header("Main Account")]
    [SerializeField] private GameObject m_MainAccountScreen;
    [Header("Select Character")]
    [SerializeField] private GameObject m_SelectCharacterUI;
    [SerializeField] private List<CharacterAppearanceData> m_CharacterList;
    [SerializeField] private Transform m_CharacterListParent;
    [SerializeField] private GameObject m_CharacterTemplate;
    [SerializeField] private TMP_Text m_CharacterName;
    [SerializeField] private List<GameObject> m_CharacterPanels;
    [SerializeField] private Button m_EnterGameBtn;
    [Header("Create Character")]
    [SerializeField] private GameObject m_CreateCharacterUI;
    [SerializeField] private TMP_Text m_SelectSkinColorTxt;
    [SerializeField] private TMP_Text m_SelectHairColorTxt;
    [SerializeField] private TMP_Text m_SelectHairStyleTxt;
    [SerializeField] private TMP_Text m_SelectFacialHairStyleTxt;
    [SerializeField] private TMP_Text m_SelectEyebrowStyleTxt;
    [SerializeField] private TMP_Text m_SelectEyeColorTxt;
    [SerializeField] private TMP_InputField m_NameInput;
    [SerializeField] private Button m_SubmitCreateCharacter;
    public CharacterBuilder CreateCharacter { get; private set; }


    protected override void Awake()
    {
        m_SubmitCreateCharacter.onClick.AddListener(async () => await SubmitCharacter());
    }

    private void Update()
    {
        if (m_NameInput.text.Length > 0)
        {
            if (!m_SubmitCreateCharacter.interactable)
                m_SubmitCreateCharacter.interactable = true;
        }
        else
        {
            if (m_SubmitCreateCharacter.interactable)
                m_SubmitCreateCharacter.interactable = false;
        }
    }

    #region FirebaseAuth/Login

    public void Login()
    {
        StartCoroutine(LoginRoutine());
    }


    private IEnumerator LoginRoutine()
    {
        m_Email.interactable = false;
        m_Password.interactable = false;
        m_ConnectingPanel.SetActive(true);
        var loginTask = AuthManager.Instance.Auth.SignInWithEmailAndPasswordAsync(m_Email.text, m_Password.text);
        yield return new WaitUntil(predicate: () => loginTask.IsCompleted);

        if(loginTask.Exception != null)
        {
            IDLogger.LogWarning($"Failed to register task with {loginTask.Exception}");
            FirebaseException ex = loginTask.Exception.GetBaseException() as FirebaseException;
            AuthError err = (AuthError)ex.ErrorCode;

            string msg = "Login Failed!";
            switch (err)
            {
                case AuthError.MissingEmail:
                    msg = "Missing Email";
                    break;
                case AuthError.MissingPassword:
                    msg = "Missing Password";
                    break;
                case AuthError.InvalidEmail:
                case AuthError.WrongPassword:
                case AuthError.UserNotFound:
                    msg = "Invalid Email or Password";
                    break;
            }
            IDLogger.LogError(msg);
            m_Email.interactable = true;
            m_Password.interactable = true;
            m_ConnectingPanel.SetActive(false);
        }
        else
        {
            AuthManager.Instance.SetUser(loginTask.Result);
            var user = AuthManager.Instance.User;
            IDLogger.Log($"User signed in successfully: {user.DisplayName} ({user.UserId})");
            m_Login.SetActive(false);
            m_ConnectingPanel.SetActive(false);
            m_MainAccountScreen.SetActive(true);
            NetworkManager.Instance.Connect();
        }
    }

    public void CreateUser()
    {
        StartCoroutine(CreateUserRoutine());
    }

    // https://www.youtube.com/watch?v=NsAUEyA2TRo // 

    private IEnumerator CreateUserRoutine()
    {
        if (Utilities.IsEmpty(m_EmailReg.text))
        {
            IDLogger.LogError("Please enter a valid username");
        }
        else if (Utilities.IsEmpty(m_PasswordReg[0].text) || Utilities.IsEmpty(m_PasswordReg[1].text))
        {
            IDLogger.LogError("Please enter your passwords");
        }
        else if (m_PasswordReg[0].text != m_PasswordReg[1].text)
        {
            IDLogger.LogError("Passwords do not match");
        }
        else
        {
            var registerTask = AuthManager.Instance.Auth.CreateUserWithEmailAndPasswordAsync(m_EmailReg.text, m_PasswordReg[0].text);
            yield return new WaitUntil(predicate: () => registerTask.IsCompleted);

            if (registerTask.Exception != null)
            {
                IDLogger.LogWarning($"Failed to register task with {registerTask.Exception}");
                FirebaseException ex = registerTask.Exception.GetBaseException() as FirebaseException;
                AuthError err = (AuthError)ex.ErrorCode;

                string msg = "Register Failed!";
                switch (err)
                {
                    case AuthError.MissingEmail:
                        msg = "Missing Email";
                        break;
                    case AuthError.MissingPassword:
                        msg = "Missing Password";
                        break;
                    case AuthError.WeakPassword:
                        msg = "Weak Password";
                        break;
                    case AuthError.EmailAlreadyInUse:
                        msg = "Email Already In Use";
                        break;
                }
                IDLogger.LogError(msg);
            }
            else
            {
                AuthManager.Instance.SetUser(registerTask.Result);
                var user = AuthManager.Instance.User;
                if(user != null)
                {
                    UserProfile profile = new UserProfile { DisplayName = m_EmailReg.text };

                    var profileTask = user.UpdateUserProfileAsync(profile);
                    yield return new WaitUntil(predicate: () => profileTask.IsCompleted);

                    if (profileTask.Exception != null)
                    {
                        IDLogger.LogWarning($"Failed to register task with {profileTask.Exception}");
                        FirebaseException ex = profileTask.Exception.GetBaseException() as FirebaseException;
                        AuthError err = (AuthError)ex.ErrorCode;
                    }
                    else
                    {
                        m_Register.SetActive(false);
                    }
                }
            }
        }
    }

    public void BackToLogin()
    {
        m_Email.interactable = true;
        m_Password.interactable = true;
        m_Login.SetActive(true);
    }


    #endregion

    #region CharacterSelect/CharacterCreate


    public void PopulateCharacters(List<CharacterAppearanceData> chars)
    {
        if(m_CharacterPanels != null && m_CharacterPanels.Count > 0)
        {
            foreach (var p in m_CharacterPanels)
            {
                Destroy(p);
            }
            m_CharacterPanels = new List<GameObject>();
        }
        m_CharacterList = chars;
        foreach(var c in m_CharacterList)
        {
            GameObject panel = Instantiate(m_CharacterTemplate, m_CharacterListParent);
            panel.GetComponent<Button>().onClick.AddListener(delegate {SelectCharacter(c);});
            var data = panel.GetComponent<CharacterSelectData>();
            data.CharName.SetText(c.Name);
            data.CharInfo.SetText($"Level {c.Level} - Total Level {c.TotalLevel}");
            m_CharacterPanels.Add(panel);
        }
    }

    private void SelectCharacter(CharacterAppearanceData c)
    {
        LobbyPlayer localPlayer = (LobbyPlayer)PlayerManager.GetLocalPlayer();
        localPlayer.SelectedCharacter = new CharacterAppearanceData(c.Name, c.Level, c.TotalLevel,
            c.SkinColor, c.HairColor, c.HairStyle, c.FacialHairStyle, c.EyebrowStyle,
            c.EyeColor, c.BootsOn, c.ShirtOn, c.PantsOn); ;
        IDLogger.Log($"Selected Character: {localPlayer.SelectedCharacter.Name}");
        localPlayer.SelectedCharacterGO.SetActive(false);
        IDLogger.Log($"Spawned Character: {localPlayer.SpawnedCharacters[c.Name]}");
        localPlayer.SelectedCharacterGO = localPlayer.SpawnedCharacters[c.Name];
        localPlayer.SelectedCharacterGO.SetActive(true);
        SetCharacterNameText(c.Name);
    }

    public void SetCharacterNameText(string name)
    {
        m_CharacterName.SetText(name);
        m_EnterGameBtn.interactable = m_CharacterName.text != "";
    }


    //CHARACTER CREATION
    public void BackToSelectCharacter(bool cancel)
    {
        LobbyPlayer player = (LobbyPlayer)PlayerManager.GetLocalPlayer();
        m_SelectCharacterUI.SetActive(true);
        m_CreateCharacterUI.SetActive(false);
        player.SelectedCharacterGO = cancel ? player.SelectedCharacterGO : CreateCharacter.gameObject;
        player.SelectedCharacterGO.SetActive(true);
        if (cancel)
            Destroy(CreateCharacter.gameObject);
    }

    public void ToCreateCharacter()
    {
        LobbyPlayer player = (LobbyPlayer)PlayerManager.GetLocalPlayer();
        if(m_CharacterPanels.Count >= 10) { IDLogger.LogError("You can only have 10 characters."); return; }
        m_SelectCharacterUI.SetActive(false);
        m_CreateCharacterUI.SetActive(true);
        player.SelectedCharacterGO?.SetActive(false);
        CreateCharacter = CharacterBuilderManager.Instance.CreateNewCharacter();
        CreateCharacter.NewRandomCharacter();
        m_SelectSkinColorTxt.text = $"{(int)CreateCharacter.CurrentSkinColor + 1}";
        m_SelectHairColorTxt.text = $"{(int)CreateCharacter.CurrentHairColor + 1}";
        m_SelectHairStyleTxt.text = $"{(int)CreateCharacter.CurrentHairStyle + 1}";
        m_SelectFacialHairStyleTxt.text = $"{(int)CreateCharacter.CurrentFacialHairStyle + 1}";
        m_SelectEyebrowStyleTxt.text = $"{(int)CreateCharacter.CurrentEyebrowStyle + 1}";
        m_SelectEyeColorTxt.text = $"{CreateCharacter.CurrentEyeColor}";
    }

    public void DeleteCharacter()
    {
        LobbyPlayer player = (LobbyPlayer)PlayerManager.GetLocalPlayer();
        foreach(var p in m_CharacterPanels)
        {
            var data = p.GetComponent<CharacterSelectData>();
            if (data.CharName.text == player.SelectedCharacter.Name)
            {
                Destroy(p);
                SetCharacterNameText("");
                PlayerManager.Instance.DeleteCharacter();
                break;
            }
        }
    }

    public void RandomName()
    {
        string randomName = HelperData.MaleFantasyNames[Random.Range(0, HelperData.MaleFantasyNames.Count)];
        while(randomName.Length > m_NameInput.characterLimit)
        {
            randomName = HelperData.MaleFantasyNames[Random.Range(0, HelperData.MaleFantasyNames.Count)];
        }
        m_NameInput.text = randomName;
    }

    public void ChangeSkinColor(bool increase)
    {
        int add = increase ? 1 : -1;
        int newSkinColor = (int)CreateCharacter.CurrentSkinColor;
        newSkinColor += add;
        if (newSkinColor < 0) newSkinColor = CreateCharacter.AppearanceSettings.BodyMats.Length - 1;
        else newSkinColor %= CreateCharacter.AppearanceSettings.BodyMats.Length;
        CreateCharacter.CurrentSkinColor = (SkinColor)newSkinColor;
        m_SelectSkinColorTxt.text = $"{(int)CreateCharacter.CurrentSkinColor + 1}";
    }

    public void ChangeHairColor(bool increase)
    {
        int add = increase ? 1 : -1;
        int newHairColor = (int)CreateCharacter.CurrentHairColor;
        newHairColor += add;
        if (newHairColor < 0) newHairColor = CreateCharacter.AppearanceSettings.HairMatsType1.Length - 1;
        else newHairColor %= CreateCharacter.AppearanceSettings.HairMatsType1.Length;
        CreateCharacter.CurrentHairColor = (HairColor)newHairColor;
        m_SelectHairColorTxt.text = $"{(int)CreateCharacter.CurrentHairColor + 1}";
    }

    public void ChangeHairStyle(bool increase)
    {
        int add = increase ? 1 : -1;
        int newHairStyle = (int)CreateCharacter.CurrentHairStyle;
        newHairStyle += add;
        //Adding an extra for Bald
        if (newHairStyle < 0) newHairStyle = CreateCharacter.AppearanceSettings.HairStyles.Length;
        else newHairStyle %= CreateCharacter.AppearanceSettings.HairStyles.Length + 1;
        CreateCharacter.CurrentHairStyle = (HairStyle)newHairStyle;
        m_SelectHairStyleTxt.text = $"{(int)CreateCharacter.CurrentHairStyle + 1}";
    }

    public void ChangeFacialHairStyle(bool increase)
    {
        int add = increase ? 1 : -1;
        int newHairStyle = (int)CreateCharacter.CurrentFacialHairStyle;
        newHairStyle += add;
        //Adding an extra for None
        if (newHairStyle < 0) newHairStyle = CreateCharacter.AppearanceSettings.BeardStyles.Length;
        else newHairStyle %= CreateCharacter.AppearanceSettings.BeardStyles.Length;
        CreateCharacter.CurrentFacialHairStyle = (FacialHairStyle)newHairStyle;
        m_SelectFacialHairStyleTxt.text = $"{(int)CreateCharacter.CurrentFacialHairStyle + 1}";
    }

    public void ChangeEyebrowStyle(bool increase)
    {
        int add = increase ? 1 : -1;
        int newHairStyle = (int)CreateCharacter.CurrentEyebrowStyle;
        newHairStyle += add;
        //Adding an extra for None
        if (newHairStyle < 0) newHairStyle = CreateCharacter.AppearanceSettings.EyebrowStyles.Length - 1;
        else newHairStyle %= CreateCharacter.AppearanceSettings.EyebrowStyles.Length;
        CreateCharacter.CurrentEyebrowStyle = (EyebrowStyle)newHairStyle;
        m_SelectEyebrowStyleTxt.text = $"{(int)CreateCharacter.CurrentEyebrowStyle + 1}";
    }

    public void ChangeEyeColor(bool increase)
    {
        int add = increase ? 1 : -1;
        int newEyeColor = (int)CreateCharacter.CurrentEyeColor;
        newEyeColor += add;
        //Adding an extra for None
        if (newEyeColor < 0) newEyeColor = CreateCharacter.AppearanceSettings.EyeMats.Length - 1;
        else newEyeColor %= CreateCharacter.AppearanceSettings.EyeMats.Length;
        CreateCharacter.CurrentEyeColor = (EyeColor)newEyeColor;
        m_SelectEyeColorTxt.text = $"{CreateCharacter.CurrentEyeColor}";
    }

    public void SetCharacterName()
    {
        CreateCharacter.Name = m_NameInput.text;
    }

    public async Task SubmitCharacter()
    {
        LobbyPlayer player = (LobbyPlayer)PlayerManager.GetLocalPlayer();
        m_SubmitCreateCharacter.interactable = false;
        if (CreateCharacter == null) CreateCharacter = CreateCharacter.GetComponent<CharacterBuilder>();
        try
        {
            await CheckCharacterExists(m_NameInput.text);
        } catch (System.Exception e)
        {
            m_SubmitCreateCharacter.interactable = true;
            IDLogger.LogError($"Tried to check but no server response with Exception {e}");
        }
        if (m_CharacterExists) { IDLogger.LogError("A character with this name already exists."); return; }
        SetCharacterName();
        CharacterAppearanceData data = new CharacterAppearanceData(CreateCharacter.Name, (byte)1, (ushort)GameManager.NUM_OF_SKILLS,
            (byte)CreateCharacter.CurrentSkinColor, (byte)CreateCharacter.CurrentHairColor, (byte)CreateCharacter.CurrentHairStyle, (byte)CreateCharacter.CurrentFacialHairStyle, (byte)CreateCharacter.CurrentEyebrowStyle,
            (byte)CreateCharacter.CurrentEyeColor, CreateCharacter.AppearanceBools[2], CreateCharacter.AppearanceBools[1], CreateCharacter.AppearanceBools[0]);
        player.AddSpawnedCharacter(data);
        m_SubmitCreateCharacter.interactable = true;
        PlayerManager.Instance.SendCreatedNewCharacter(data);
        BackToSelectCharacter(false);
        m_NameInput.text = "";
    }

    public void EnterGame()
    {
        LobbyPlayer localPlayer = (LobbyPlayer)PlayerManager.GetLocalPlayer();
        GameManager.Instance.LoadLevelPersistCharacter(localPlayer.SelectedCharacterGO, localPlayer.Email, localPlayer.SelectedCharacter.Name);
    }

    #endregion

    #region Messages
    //-------------------------------------------------------------------------------------//
    //---------------------------- Message Sending ----------------------------------------//
    //-------------------------------------------------------------------------------------//

    public void SendName()
    {
        Message msg = Message.Create(MessageSendMode.Reliable, (ushort)ClientToServerId.AccountInformation);
        msg.AddString(m_Email.text);
        NetworkManager.Instance.Client.Send(msg);
    }

    private static TaskCompletionSource<bool> m_WaitingForServerResponse;
    private static bool m_CharacterExists = true;

    public async Task CheckCharacterExists(string charName)
    {
        IDLogger.Log("Checking if character exists");
        m_CharacterExists = true;
        Message msg = Message.Create(MessageSendMode.Reliable, (ushort)ClientToServerId.CheckCharacterExists);
        msg.AddString(charName);
        NetworkManager.Instance.Client.Send(msg);
        m_WaitingForServerResponse = new TaskCompletionSource<bool>();
        IDLogger.Log("Waiting for server");
        await m_WaitingForServerResponse.Task;
        IDLogger.Log($"Server responded {m_CharacterExists}");
    }

    //-------------------------------------------------------------------------------------//
    //---------------------------- Message Handlers ---------------------------------------//
    //-------------------------------------------------------------------------------------//

    [MessageHandler((ushort)ServerToClientId.ReceiveCharacterExists)]
    private static void ReceiveCharacterExists(Message msg)
    {
        m_CharacterExists = msg.GetBool();
        m_WaitingForServerResponse.SetResult(true);
    }

    #endregion


    #region Debug
    public void DebugButton()
    {
        IDLogger.Log("Button works");
    }
    #endregion
}