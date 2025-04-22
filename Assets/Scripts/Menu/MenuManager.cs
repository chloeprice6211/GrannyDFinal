using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Mirror;
using Unity.VisualScripting;
using UnityEngine.Rendering.PostProcessing;
using Steamworks;
using UnityEngine.Localization.Components;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using Cinemachine;
using UnityEngine.Events;

public class MenuManager : MonoBehaviour
{
    #region vars

    public static MenuManager instance;

    [SerializeField] SetupPanel setupPanel;
    public enum MenuPoint
    {
        Main,
        Setup,
        Coop,
        Settings,
        Ending
    }
    [HideInInspector]
    public MenuPoint currentMenuPoint;

    [Header("Audio Clips")]
    [SerializeField] AudioClip laughing;
    [SerializeField] AudioClip singing;
    [SerializeField] AudioClip penWriting;
    PlayerControls _controls;

    [SerializeField] AudioSource rainingAudioSource;
    [SerializeField] AudioSource roomAmbientSource;
    [SerializeField] AudioSource ghostAmbienceSource;

    [SerializeField] List<AudioSource> AdjustedAudioSources;
    [SerializeField] float[] MaxVolumeArray;

    [SerializeField] ColorGrading _grading;

    public Transform playersHolder;

    [Header("UI")]
    [SerializeField] Button returnToMenuButton;
    public Image menuButtonsPanel;
    [SerializeField] Button startCoopButton;
    [SerializeField] Button proceedButton;
    [SerializeField] public List<GameObject> uiHolders;
    [SerializeField] Button playButton;
    [SerializeField] EndingPanel endingPanel;
    public GameObject clientLoadingPanel;
    public GameObject settingsPanel;
    public PostProcessVolume ppv;
    [SerializeField] AudioSource UIaudioSource;

    [Header("multiplayer")]
    [SerializeField] Button inviteFriendsButton;
    [SerializeField] Canvas coopUI;
    public TextMeshProUGUI loggerText;
    public TextMeshProUGUI lobbyNameText;

    [Header("Ambience")]
    [SerializeField] Transform ghostPosition;
    [SerializeField] AudioSource TVAudio;

    [Header("Camera")]
    public Transform cameraFollowTarget;
    public List<Transform> cameraPositions;

    [Header("Animations")]
    public Animation screenFadeAnimation;
    [SerializeField] AnimationClip changingPointScreenFadeClip;
    [SerializeField] Animation setupLampBlinkingAnimation;

    [Header("Interior components")]
    [SerializeField] GameObject TV;

    public Texture2D clickPointerTexture;
    public Texture2D regularPointerTexture;

    private float timeElapsed = 0;
    private bool hasSinged;

    private GameObject _currentUIHolder;
    private AsyncOperation asyncOp;
    public LocalizeStringEvent stringEvent;

    [Header("Message")]
    [SerializeField] TextMeshProUGUI messageTextField;
    [SerializeField] GameObject panel;
    [SerializeField] Image messageTypeIcon;
    [SerializeField] List<Sprite> messageIconSprites;
    [SerializeField] Animation messageAnimation;
    [SerializeField] AnimationClip messageAppearClip;
    [SerializeField] AnimationClip messageDissappearClip;
    bool _isWarningMessageActive;

    private bool _isChangingPointRoutineOngoing;
    public Level level;
    private LobbyPlayer _lobbyPlayer;
    [HideInInspector]
    public bool isHost;

    [HideInInspector]
    public TargetMenuPoint arrivalPoint;

    public bool displaySubs = true;

    //events
    public static UnityEvent<Locale> OnLocaleInitialized = new();

    #endregion

    #region unity callbacks

    private void Awake()
    {
        instance = this;
        currentMenuPoint = MenuPoint.Main;
        _controls = new();
        _controls.Disable();

    }
    private void Start()
    {
        CustomNetworkManager.Instance.LobbyPlayers.Clear();
        CustomNetworkManager.Instance.GamePlayers.Clear();

        SetupMenuPoints();
        StartCoroutine(IncreaseRainVolumeRoutine());

        CustomNetworkManager.Instance.StopClient();
        CustomNetworkManager.Instance.StopHost();

        _currentUIHolder = uiHolders[0];
        ppv.profile.TryGetSettings<ColorGrading>(out _grading);
        ApplyBrightness();

        AdjustedAudioSources[2].volume = MaxVolumeArray[2] * PrefsSettings.s_mainMenuAmbientCap;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.lockState = CursorLockMode.None;

        Cursor.visible = false;

        if (Ending.ending.endingType != Ending.EndingType.None)
        {
            TranslateToEndingPoint();
        }

        //var translatedValue = LocalizationSettings.StringDatabase.GetLocalizedString("characterUiMessage", "lang");

        //Debug.Log(translatedValue);

        StartCoroutine(LocalizationInitRoutine());
    }


    IEnumerator LocalizationInitRoutine()
    {
        while (LocalizationSettings.AvailableLocales.Locales.Count < 3)
            yield return null;

        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.GetLocale(PlayerPrefs.GetString("language"));
        if(OnLocaleInitialized!=null)
        OnLocaleInitialized?.Invoke(LocalizationSettings.SelectedLocale);


    }

    void Update()
    {
        timeElapsed += Time.deltaTime;

        if (timeElapsed > 100 && !hasSinged)
        {
            AudioSource.PlayClipAtPoint(singing, ghostPosition.position);
            hasSinged = true;
        }
        if (timeElapsed > 110)
        {
            timeElapsed = 0;
        }
    }

    #endregion

    #region Buttons handler

    #region consolevars
    public string mapName;
    #endregion

    public void OnCoopClick(Level level)
    {
        if (_isChangingPointRoutineOngoing) return;

        SetupPanel.Instance.levelToGo = level;
        //Debug.Log(level.sceneName);
        this.level = level;
        LevelSettings.ghostHuntDuration = level.huntTime;

        CustomNetworkManager.Instance.spawnPositions = new();
        foreach (Transform spawn in level.spawnPoints)
        {
            //Debug.Log(level.spawnPoints[0]);
            CustomNetworkManager.Instance.spawnPositions.Add(spawn.localPosition);
            CustomNetworkManager.Instance.spawnRotations.Add(spawn.localRotation);
        }

        isHost = true;
        ChangeMenuPoint(new TargetMenuPoint(MenuPoint.Coop, instance.uiHolders[2], instance.cameraPositions[2]));
    }

    public void OnBackClick()
    {
        if (_isChangingPointRoutineOngoing) return;

        ReturnToMainMenu();
    }

    #endregion

    public void ReturnToMainMenu()
    {
        ChangeMenuPoint(TargetMenuPoint.Menu);
    }

    public void ChangeMenuPoint(TargetMenuPoint point)
    {
        StartCoroutine(ChangeMenuPointRoutine(point));
    }

    private IEnumerator ChangeMenuPointRoutine(TargetMenuPoint point)
    {
        _isChangingPointRoutineOngoing = true;
        screenFadeAnimation.Play(changingPointScreenFadeClip.name);

        if(Application.isEditor) yield return new WaitForSeconds(0.1f);
        else yield return new WaitForSeconds(1.15f);
        yield return new WaitForSeconds(1.15f);
        yield return new WaitForSeconds(0.1f);
        _isChangingPointRoutineOngoing = false;

        if (point.menuPoint != MenuPoint.Main)
        {
            //returnToMenuButton.gameObject.SetActive(true);
        }

        switch (currentMenuPoint)
        {
            case MenuPoint.Setup:
                NetworkManager.singleton.StopHost();
                isHost = false;

                break;
            case MenuPoint.Coop:
                if (isHost)
                {
                    NetworkManager.singleton.StopHost();
                }
                else
                {
                    NetworkManager.singleton.StopClient();
                }

                isHost = false;

                break;
        }
        switch (point.menuPoint)
        {
            case MenuPoint.Setup:

                setupPanel.SetupCoopScreen(false);

                if (!setupLampBlinkingAnimation.isPlaying)
                {
                    setupLampBlinkingAnimation.Play();
                }
 
                //CustomNetworkManager.Instance.StartHost();
                NetworkServer.maxConnections = 1;

                break;

            case MenuPoint.Coop:

                if (!setupLampBlinkingAnimation.isPlaying)
                {
                    setupLampBlinkingAnimation.Play();
                }
                setupPanel.SetupCoopScreen(true);

                if (isHost)
                {
                    NetworkServer.maxConnections = 2;
                CustomNetworkManager.Instance.StartHost();
                    SetupPanel.Instance.SetupServerCoopScreen();
                }
                else
                {
                    SetupPanel.Instance.SetupClientCoopScreen();
                }

                CustomNetworkManager.Instance.UpdateLobbyPlayers();

                break;

            case MenuPoint.Main:
                returnToMenuButton.gameObject.SetActive(false);
                isHost = false;

                break;

            case MenuPoint.Ending:

                endingPanel.ShowEndingPanel(Ending.ending);
                Ending.ending.endingType = Ending.EndingType.None;
                break;
        }

        currentMenuPoint = point.menuPoint;

        _currentUIHolder.SetActive(false);
        _currentUIHolder = point.uiHolder;

        cameraFollowTarget.localPosition = point.cameraPosition.position;
        cameraFollowTarget.localRotation = point.cameraPosition.rotation;

        point.uiHolder.SetActive(true);
        currentMenuPoint = point.menuPoint;
    }


    private IEnumerator IncreaseRainVolumeRoutine()
    {
        float _targetVolume = 0.11f;
        float _currentVolume = 0;
        float rate = 0.005f;

        while (_currentVolume <= _targetVolume)
        {
            _currentVolume += rate;
            rainingAudioSource.volume = _currentVolume;
            roomAmbientSource.volume = _currentVolume * 8;
            ghostAmbienceSource.volume = _currentVolume * 3f;


            yield return new WaitForSeconds(.1f);
        }

        Cursor.visible = true;
    }

    void SetupMenuPoints()
    {
        TargetMenuPoint._menuPoint = new(MenuPoint.Main,
       instance.uiHolders[0],
       instance.cameraPositions[0]);

        TargetMenuPoint._setupPoint = new(MenuPoint.Setup,
        instance.uiHolders[1],
        instance.cameraPositions[1]);

        TargetMenuPoint._coopPoint = new(MenuPoint.Coop,
        instance.uiHolders[2],
        instance.cameraPositions[2]);

        TargetMenuPoint._endingPoint = new(MenuPoint.Ending,
            instance.uiHolders[3],
            instance.cameraPositions[3]);
    }

    void TranslateToEndingPoint()
    {
        ChangeMenuPoint(TargetMenuPoint._endingPoint);
    }

    public void ShowMessage(string message, MessageType type)
    {
        if (_isWarningMessageActive)
        {
            return;
        }

        _isWarningMessageActive = true;

        messageTextField.text = message;
        //messageTypeIcon.sprite = messageIconSprites[0];

        messageAnimation.Play(messageAppearClip.name);
        Invoke("ShowDelayed", 4f);

    }
    void ShowDelayed()
    {
        _isWarningMessageActive = false;
        messageAnimation.Play(messageDissappearClip.name);
    }

    public void ApplyVolume(float ratio)
    {
        for (int a = 0; a < AdjustedAudioSources.Count; a++)
        {
            AdjustedAudioSources[a].volume = MaxVolumeArray[a] * ratio;
        }

        ApplyBrightness();
    }

    public void ApplyBrightness()
    {
        _grading.postExposure.value = PrefsSettings.s_postExposureMax + (PrefsSettings.s_postExposure * 2);
    }

    public void PlaySound(AudioClip sound, float volume)
    {
        UIaudioSource.PlayOneShot(sound, volume);
    }

    #region console
    [QFSW.QC.Command("load")]
    void LoadMap(string _mapName){
        mapName = _mapName;
        StartCoroutine(ConsoleLoadRoutine());
    }

    IEnumerator ConsoleLoadRoutine(){
        Level level;
        switch(mapName){
            case "1":
            level = SetupPanel.Instance.consoleLevelsList[0];
            break;
            case "2":
            level = SetupPanel.Instance.consoleLevelsList[1];
            break;
            case "3":
            level = SetupPanel.Instance.consoleLevelsList[2];
            break;
            case "4":
            level = SetupPanel.Instance.consoleLevelsList[3];
            break;
            default:
            level = SetupPanel.Instance.consoleLevelsList[0];
            break;

        }
        OnCoopClick(level);
        yield return new WaitForSeconds(0.1f);
        SetupPanel.Instance.LaunchMapConsole();
    }
    #endregion
}
public enum MessageType
{
    Warning,
    Error,
    Success
}


