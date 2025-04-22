using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Steamworks;
using Mirror;
using System;
using UnityEngine.Localization.Settings;
using Cinemachine;

public class SetupPanel : MonoBehaviour
{
    #region vars

    private static SetupPanel _instance;

    [Header("UI")]
    [SerializeField] Button playButton;
    [SerializeField] TextMeshProUGUI playButtonText;
    [SerializeField] GameObject clientSetupPanel;
    [SerializeField] GameObject levelSetupPanel;
    [SerializeField] TipPanel tipPanel;
    [SerializeField] UICheckBox highlightCheckbox;
    [SerializeField] TextMeshProUGUI levelNameField;

    [Header("loading UI")]
    [SerializeField] Slider loadingProgressSlider;
    [SerializeField] TextMeshProUGUI loadingProgressNumberTextField;
    [SerializeField] GameObject loadingPanel;

    [Header("setup screen")]
    [SerializeField] List<Renderer> footagesRenderers;
    [SerializeField] List<Material> footageMaterials;
    [SerializeField] bool isSteam;

    [Header("Coop Screen")]
    [SerializeField] Renderer _coopScreenRenderer;
    [SerializeField] List<Material> coopScreenIndicatorMaterials;
    [SerializeField] Light coopScreenLightSource;
    [SerializeField] GameObject coopActivePanel;
    [SerializeField] GameObject coopInactivePanel;
    [SerializeField] TextMeshProUGUI playerCountText;
    [SerializeField] Button inviteButton;
    [SerializeField] TextMeshProUGUI inviteButtonText;
    public List<SwitchableButton> difficultyButtons;
    [SerializeField] List<SwitcherButton> levelSwitchButtons;

    public List<Level> consoleLevelsList;

    private Level _currentDisplayedLevel;
    //private bool _isDifficultyChanged;

    public Level levelToGo;
    public static LevelSettings LevelSettings;
    public static GameMode GameMode;
    public static bool isOutlineAllowed;

    public bool hasHighlightBeenChanged;

    public CinemachineVirtualCamera virtualCamera;
    [SerializeField] Animation dragAnimation;

    [SerializeField] Animation itemsButtonAnimation;

    public static SetupPanel Instance
    {
        get
        {
            return _instance;
        }
        set
        {
            if (_instance == null)
            {
                _instance = new();
            }
        }
    }

    #endregion

    #region unity callbacks

    private void Awake()
    {
        _instance = this;
    }

    #endregion

    #region button handlers

    public void SwitchToItemsGuide()
    {
        dragAnimation.Play();
        virtualCamera.gameObject.SetActive(true);

        if (itemsButtonAnimation.isPlaying)
            Invoke("StopAnimations", 1f);

    }

    void StopAnimations()
    {
        itemsButtonAnimation.Stop();
        itemsButtonAnimation.gameObject.GetComponent<Outline>().enabled = false;
    }

    public void ReturnFromItemsGuide()
    {
        virtualCamera.gameObject.SetActive(false);
    }

    public void OnDifficultyButtonClick()
    {
        //_isDifficultyChanged = true;
        playButton.gameObject.SetActive(true);
    }

    public void OnPlayButtonClick()
    {
        if(CustomNetworkManager.Instance.LobbyPlayers.Count > levelToGo.playerCap)
        {
            MenuManager.instance.ShowMessage("Max players: " + levelToGo.playerCap, MessageType.Warning);
            Debug.Log("too many players");

            return;
        }

        LobbyNetworkManager.instance.CmdChangeDifficulty(LevelSettings);
        LoadScene(levelToGo);
    }

    public void OnInviteButtonClick()
    {
        SteamFriends.ActivateGameOverlayInviteDialog(SteamLobby.instance._lobbyId);
    }

    public void OnHighlightClick()
    {
        isOutlineAllowed = highlightCheckbox.isChecked;
    }

    #endregion

    #region Functionality

    public void SetupCoopScreen(bool isCoop)
    {
        if(PlayerPrefs.GetInt("tip") == 0)
        {
            itemsButtonAnimation.Play();
            PlayerPrefs.SetInt("tip", 1);
        }
        
        if (isCoop)
        {
            if (coopInactivePanel.activeInHierarchy)
            {
                coopInactivePanel.SetActive(false);
            }

            GameMode = GameMode.Coop;
            _coopScreenRenderer.material = coopScreenIndicatorMaterials[0];
            coopScreenLightSource.color = Color.green;
            coopActivePanel.SetActive(true);
        }
        else
        {
            GameMode = GameMode.Solo;
            clientSetupPanel.SetActive(false);
            if (coopActivePanel.activeInHierarchy)
            {
                coopActivePanel.SetActive(false);
            }

            _coopScreenRenderer.material = coopScreenIndicatorMaterials[1];
            coopScreenLightSource.color = Color.red;
            coopInactivePanel.SetActive(true);
        }
    }

    public void SetupClientCoopScreen()
    {
        inviteButton.gameObject.SetActive(false);
        clientSetupPanel.SetActive(true);

        foreach (SwitchableButton button in difficultyButtons)
        {
            button.GetComponent<EventTrigger>().enabled = false;
        }
    }

    public void SetupServerCoopScreen()
    {
        //OnHighlightClick();
        //Debug.Log(levelToGo.levelName);
        //  if(isSteam){
        //       SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, 4);
        //  }
        //  else{
        //     CustomNetworkManager.Instance.StartHost();
        //  }

         //SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, 4);
          CustomNetworkManager.Instance.StartHost();
       
        //
        //inviteButton.gameObject.SetActive(true);
        clientSetupPanel.SetActive(false);

        foreach (SwitchableButton button in difficultyButtons)
        {
            button.GetComponent<EventTrigger>().enabled = true;
        }
    }

    private void LoadScene(Level level)
    {
        levelSetupPanel.SetActive(false);
        loadingPanel.SetActive(true);

        LobbyNetworkManager.instance.CmdEnablePreSceneFade();
        NetworkManager.singleton.ServerChangeScene(MenuManager.instance.level.sceneName);
        StartCoroutine(LoadSceneRoutine());
    }

    public void ApplyLevelName(string level, string footageIndexText)
    {
        Debug.Log(level);
        int footageIndex = Convert.ToInt32(footageIndexText);

        if(level != null)
        {
            levelNameField.text = level;
        }

        footagesRenderers[0].material = footageMaterials[footageIndex * 2];
        footagesRenderers[1].material = footageMaterials[(footageIndex * 2) + 1];
    }
   

    IEnumerator LoadSceneRoutine()
    {
        NetworkManager.loadingSceneAsync.allowSceneActivation = false;

        while (!NetworkManager.loadingSceneAsync.isDone)
        {
            loadingProgressSlider.value = Mathf.Lerp(loadingProgressSlider.value,
                (NetworkManager.loadingSceneAsync.progress * loadingProgressSlider.maxValue),
                Time.deltaTime * 35);
            loadingProgressNumberTextField.text = loadingProgressSlider.value.ToString("0.00") + "%";

            if (NetworkManager.loadingSceneAsync.progress >= .9f)
            {
                loadingProgressSlider.value += loadingProgressSlider.maxValue;
                loadingProgressNumberTextField.text = "100%";

                break;
            }

            yield return null;
        }

        MenuManager.instance.screenFadeAnimation.Play("ExitFade");
        yield return new WaitForSeconds(2f);
        NetworkManager.loadingSceneAsync.allowSceneActivation = true;

    }

    #endregion

    #region Event handlers

    public void UpdatePlayerCount(int num)
    {
        if (!MenuManager.instance.isHost) return;

       
    }

    #endregion

    #region console
    public void LaunchMapConsole(){
        
        StartCoroutine(CoroutineOne());

        
    }   

    IEnumerator CoroutineOne(){
        SetupServerCoopScreen();
        while(!NetworkClient.isConnected || !NetworkClient.ready || NetworkClient.isConnecting){
            yield return null;
        }
        
        LobbyNetworkManager.instance.CmdChangeDifficulty(LevelSettings.Easy);
        //yield return new WaitForSeconds(.5f);
        Debug.Log("loading " + MenuManager.instance.level.levelName);
         LoadScene(MenuManager.instance.level);
         
    }

    void TestOne(){
        
        //Level level;
        // switch(levelname){
        //     case "brown":
        //     level = consoleLevelsList[0];
        //     break;
        // default:
        // level = consoleLevelsList[0];
        // break;
        // }

        LoadScene(consoleLevelsList[0]);
    }
    #endregion

}

public struct LevelSettings
{
    public LevelDifficultyType DifficultyType;
    public GameMode GameMode;

    //pills configuration
    public int PillsAmount;
    public int PillsEffectDuration;

    public bool IsOutlineAllowed;

    //ghost events
    public int GhostEventCooldown;

    // time to event is random number from [var] and [var + (var/2)]
    public int GhostWalkModeTime;
    public int GhostAppearanceTime;
    public int GhostHuntTime;
    public int GhostAmbientSound;
    public int GhostActionTime;
    public int GhostCornerAppearanceTime;

    public int GhostStartedCooldown;

    public float GhostSpeed;
    public int SealLockerDuringHuntChance;

    public static int ghostHuntDuration = 20;
    public static float ghostSpeedMulitplier = 1f;

    private static LevelSettings _easy = new()
    {
        DifficultyType = LevelDifficultyType.Easy,

        PillsAmount = 5,
        PillsEffectDuration = 50,

        GhostEventCooldown = 45,

        GhostWalkModeTime = 375,
        GhostAppearanceTime = 75,
        GhostCornerAppearanceTime = 85,

        GhostHuntTime = 110,

        GhostAmbientSound = 35,
        GhostActionTime = 30,

        GhostStartedCooldown = 95,

        GhostSpeed = 1f,
        SealLockerDuringHuntChance = 0

        #region test

        // DifficultyType = LevelDifficultyType.Easy,

        //PillsAmount = 5,
        //PillsEffectDuration = 50,

        //GhostEventCooldown = 45,
        //GhostEventCooldown = 5,

        //GhostWalkModeTime = 375,
        //GhostAppearanceTime = 75,
        //GhostCornerAppearanceTime = 85,

        //GhostHuntTime = 110,
        //GhostHuntTime = 2,

        //GhostAmbientSound = 35,
        //GhostActionTime = 30,

        //GhostStartedCooldown = 95,
        //GhostStartedCooldown = 1,

        //GhostSpeed = 1f,
        //SealLockerDuringHuntChance = 0

        #endregion

    };
    private static LevelSettings _normal = new()
    {
        DifficultyType = LevelDifficultyType.Normal,

        PillsAmount = 3,
        PillsEffectDuration = 30,

        GhostEventCooldown = 30,
        GhostStartedCooldown = 60,

        GhostCornerAppearanceTime = 75,
        GhostWalkModeTime = 300,
        GhostAppearanceTime = 70,
        GhostHuntTime = 90,
        GhostAmbientSound = 25,
        GhostActionTime = 16,

        GhostSpeed = 1.5f,
        SealLockerDuringHuntChance = 35
    };
    private static LevelSettings _nightmare = new()
    {
        DifficultyType = LevelDifficultyType.Nightmate,

        PillsAmount = 1,
        PillsEffectDuration = 20,

        GhostEventCooldown = 15,

        GhostStartedCooldown = 25,
        GhostCornerAppearanceTime = 30,
        GhostWalkModeTime = 250,
        GhostAppearanceTime = 35,
        GhostHuntTime = 40,
        GhostAmbientSound = 17,
        GhostActionTime = 8,

        GhostSpeed = 2f,
        SealLockerDuringHuntChance = 80

    };

    public static LevelSettings Easy
    {
        get
        {
            return _easy;
        }
    }
    public static LevelSettings Normal
    {
        get{
            return _normal;
        }
    }
    public static LevelSettings Nightmare
    {
        get
        {
            return _nightmare;
        }
    }
}


public enum LevelDifficultyType
{
    Easy,
    Normal,
    Nightmate
}
public enum GameMode
{
    Solo,
    Coop
}
