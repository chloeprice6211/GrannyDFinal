using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using System.Text;
using Mirror;
using UnityEngine.Localization.Settings;

public enum Keys
{
    E,
    F,
    Space,
    F1,
    Q,
    W
}

public class UIManager : MonoBehaviour
{
    private static UIManager _instance;

    [Header("Components")]
    public Animation crosshairAnimation;
    public ControlsMessageTip controlsMessageTip;
    [SerializeField] TextMeshProUGUI characterMessageTextField;
    [SerializeField] TextMeshProUGUI controlsMessageTetxField;
    [SerializeField] Image crosshair;
    [SerializeField] Sprite[] buttons;
    public DeathPanel endingPanel;
    [SerializeField] GameObject pausePanel;
    public GameObject waitingForPlayersPanel;
    public Sprite interactSprite;
    [SerializeField] GameObject highlightTipPanel;
    [SerializeField] Animation highlightTipAnimation;
    public Image preDeathGlitchImage;
    [SerializeField] TextMeshProUGUI fadeTipText;
    [SerializeField] Animation fadeTipAnimation;
    public UiInventory uiInventory;

    [Header("Interact canvases")]
    public Canvas interactInfo;
    public Canvas alternateInteractInfo;

    [Header("No ray interactions")]
    [SerializeField] Image noRayInteractionButton;
    [SerializeField] Image noRayInteractionHolder;
    [SerializeField] Image noRayInteractionAlternateHolder;
    [SerializeField] Image noRayAlternateInteractionButton;

    [Header("Interactions texts")]
    [SerializeField] TextMeshProUGUI noRayAlternateInteractOption;
    [SerializeField] TextMeshProUGUI additionalText;
    [SerializeField] TextMeshProUGUI noRayInteractOption;

    [Header("Inspect")]
    public TextMeshProUGUI inspectLabelTextField;
    public TextMeshProUGUI inspectDescriptionTextField;
    public GameObject inspectInformationHolder;
    [SerializeField] Animation inspectFadeAnimation;
    [SerializeField] TextMeshProUGUI inspectButtonText;
    [SerializeField] TextMeshProUGUI inspectButtonLabel;
    [SerializeField] Image inspectBG;
    [SerializeField] Animation inspectHolderBGAnimation;
    public AudioSource inspectTypeSource;
    [SerializeField] Image inspectButtonBorder;

    [Header("READER")]
    public NotesReader Reader;

    [SerializeField] TextMeshProUGUI interactAction;
    [SerializeField] TextMeshProUGUI alternateInteractAction;

    public Animation fadeAnimation;
    public AnimationClip startFadeClip;
    public AnimationClip endFadeClip;

    public bool CanDisplayInteraction;

    private bool _isMessageRoutineOngoing = false;
    private List<string> messagesQueue = new();
    List<string> audioClipKeyQueue = new();

    string _currentDisplayed;

    public TextMeshProUGUI centralTipTextField;

    Coroutine printMessageRoutine;

    [SerializeField] AudioSource onItemHoverAudioSource;
    [SerializeField] Animation interactAnimation;
    //bool _hasAnimationPlayed;

    public List<string> UIRayKeys;
    public List<string> UIRayText;
    public List<string> UIRayToolKeys;
    public List<string> UIRayToolText;

    [SerializeField] AudioSource characterLinesSource;

    [SerializeField] AudioSource generalAudioSource;

    //prefs
    public bool displaySubsPrefValue;
    public string voiceLanguagePrefValue;

    [SerializeField] GameObject centralTip;

    public AudioSource UIAudioSource;

    float _currentLength;

    public static UIManager Instance
    {
        get
        {
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }

        InitiateLocalizationArray();
    }

    void Start()
    {
        fadeAnimation.Play(startFadeClip.name);
        _instance = this;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        LocalizationSettings.SelectedLocaleChanged += LocalizationSettings_SelectedLocaleChanged;

        displaySubsPrefValue = PlayerPrefs.GetInt("hasSubs") == 1 ? true : false;

    }

    private void LocalizationSettings_SelectedLocaleChanged(UnityEngine.Localization.Locale obj)
    {
        InitiateLocalizationArray();
    }

    void InitiateLocalizationArray()
    {
        for(int a = 0; a < UIRayKeys.Count; a++)
        {
            UIRayText[a] = LocalizationSettings.StringDatabase.GetLocalizedString("uiManager", UIRayKeys[a]);
        }
        for(int a = 0; a < UIRayToolKeys.Count; a++)
        {
            UIRayToolText[a] = LocalizationSettings.StringDatabase.GetLocalizedString("uiManager", UIRayToolKeys[a]);
        }
    }

    #region Character messages

    public void Message(string message, string audioClipKey = null)
    {
        if (string.IsNullOrEmpty(message)) return;
        StartCoroutine(DelayMessageRoutine(message, audioClipKey));
    }

    IEnumerator DelayMessageRoutine(string message, string audioClipKey)
    {
        yield return new WaitForSeconds(.4f);

        DisplayDelayedMessage(message, audioClipKey);
    }

    void DisplayDelayedMessage(string message, string audioClipKey)
    {
        if (string.IsNullOrEmpty(audioClipKey)) audioClipKey = "nullreference";
        message = LocalizationSettings.StringDatabase.GetLocalizedString("characterUiMessage", message);

        if (message == "null") return;

        if (PrefsSettings.s_hasSubs == 0) return; 

        messagesQueue.Add(message);
        audioClipKeyQueue.Add(audioClipKey);

        if (messagesQueue.Count > 1)
        {
            if (message == messagesQueue[messagesQueue.Count - 2])
            {
                messagesQueue.RemoveAt(messagesQueue.Count - 1);
               audioClipKeyQueue.RemoveAt(audioClipKeyQueue.Count - 1);
            }
        }

        if (_isMessageRoutineOngoing)
        {
            return;
        }

        StartCoroutine(MessageCoroitone(messagesQueue[0], audioClipKeyQueue[0]));
    }

    private IEnumerator MessageCoroitone(string message, string characterLineKey)
    {
        string goingText = string.Empty;
        int charIndex = 0;
        float messageDisplayLength;

        AudioClip characterLine = LocalizationSettings.AssetDatabase.GetLocalizedAsset<AudioClip>
           ("characterLines", characterLineKey, LocalizationSettings.AvailableLocales.GetLocale(PrefsSettings.s_voiceLanguage));

        if (characterLine != null)
        {
            characterLinesSource.PlayOneShot(characterLine, PrefsSettings.s_voiceVolume / .7f);
            messageDisplayLength = characterLine.length;
        }

        messageDisplayLength = characterLine == null ?
            (characterMessageTextField.text.Length / 20 + 1) :
            characterLine.length - 0.02f;

        _isMessageRoutineOngoing = true;

        

        while (charIndex != message.Length)
        {
            characterMessageTextField.text += message[charIndex];
            charIndex++;

            yield return new WaitForSeconds(.02f);
        }

        

        yield return new WaitForSeconds(messageDisplayLength);
        StartCoroutine(EraseMessageCoroutine());

    }

    private IEnumerator EraseMessageCoroutine()
    {
        while (characterMessageTextField.text.Length != 0)
        {
            characterMessageTextField.text = characterMessageTextField.text.Remove(characterMessageTextField.text.Length - 1);

            yield return new WaitForSeconds(.007f);
        }

        _isMessageRoutineOngoing = false;
        ShiftMessageQueueArray();

    }

    private void ShiftMessageQueueArray()
    {
        if (messagesQueue.Count > 1)
        {
            for (int a = 0; a < messagesQueue.Count - 1; a++)
            {
                messagesQueue[a] = messagesQueue[a + 1];
                audioClipKeyQueue[a] = audioClipKeyQueue[a + 1];
            }

            messagesQueue.RemoveAt(messagesQueue.Count - 1);
            audioClipKeyQueue.RemoveAt(audioClipKeyQueue.Count - 1);

            StartCoroutine(MessageCoroitone(messagesQueue[0], audioClipKeyQueue[0]));
        }
        else
        {
            messagesQueue.RemoveAt(messagesQueue.Count - 1);
            audioClipKeyQueue.RemoveAt(audioClipKeyQueue.Count - 1);
        }
    }

    #endregion

    #region controls tip message



    #endregion

    #region Interactions

    public void ShowInteractOption(string option)
    {
        interactInfo.gameObject.SetActive(true);
        interactAction.text = option;
    }

    public void ShowInteractOption(string gameObject, int temp)
    {
        if (UIRayText == null) return;

        if (gameObject == "Item")
        {
            interactInfo.gameObject.SetActive(true);
            interactAction.text = UIRayText[0];
        }
        else if (gameObject == "OpenDoor")
        {
            interactInfo.gameObject.SetActive(true);
            interactAction.text = UIRayText[1];
        }
        else if (gameObject == "ClosedDoor")
        {
            interactInfo.gameObject.SetActive(true);
            interactAction.text = UIRayText[2];
        }
        else if (gameObject == "ClosedDrawer")
        {
            interactInfo.gameObject.SetActive(true);
            interactAction.text = UIRayText[3];
        }
        else if (gameObject == "OpenDrawer")
        {
            interactInfo.gameObject.SetActive(true);
            interactAction.text = UIRayText[4];
        }
        else if (gameObject == "Note")
        {
            interactInfo.gameObject.SetActive(true);
            interactAction.text = UIRayText[5];
        }
        else if (gameObject == "LightOn")
        {
            interactInfo.gameObject.SetActive(true);
            interactAction.text = UIRayText[6];
        }
        else if (gameObject == "LightOff")
        {
            interactInfo.gameObject.SetActive(true);
            interactAction.text = UIRayText[7];
        }
        else if (gameObject == "CarSeat")
        {
            interactInfo.gameObject.SetActive(true);
            interactAction.text = UIRayText[8];
        }
        else if (gameObject == "CarExit")
        {
            interactInfo.gameObject.SetActive(true);
            interactAction.text = UIRayText[9];
        }
        else if (gameObject == "Button")
        {
            interactInfo.gameObject.SetActive(true);
            interactAction.text = UIRayText[10];
        }
        else if (gameObject == "Activate")
        {
            interactInfo.gameObject.SetActive(true);
            interactAction.text = UIRayText[11];
        }
        else if (gameObject == "FocusObject")
        {
            interactInfo.gameObject.SetActive(true);
            interactAction.text = UIRayText[12];
        }
        else
        {
            HideInteractOption();
        }

        //if (!_hasAnimationPlayed)
        //{
            if (!onItemHoverAudioSource.isPlaying)
                onItemHoverAudioSource.Play();


            interactAnimation.Play("InteractInfoOn");

            //_hasAnimationPlayed = true;
       // }
    }

    public void RemoveReturnInteractInfo()
    {
        
    }

    public void HideInteractOption()
    {
        _currentDisplayed = string.Empty;
        interactInfo.gameObject.SetActive(false);
        alternateInteractInfo.gameObject.SetActive(false);

        interactAnimation.Play("InteractInfoOff");
        //_hasAnimationPlayed = false;
    }

    public void ShowNoRayInteraction(string text, Keys key)
    {
        noRayInteractionButton.preserveAspect = true;
        noRayInteractionHolder.gameObject.SetActive(true);

        switch (key)
        {
            case Keys.E:
                noRayInteractionButton.sprite = buttons[0];
                break;
            case Keys.F:
                noRayInteractionButton.sprite = buttons[1];
                break;
            case Keys.Space:
                noRayInteractionButton.sprite = buttons[2];
                break;
            case Keys.F1:
                noRayInteractionButton.sprite = buttons[3];
                break;
            case Keys.Q:
                noRayInteractionButton.sprite = buttons[4];
                break;
            case Keys.W:
                noRayInteractionButton.sprite = buttons[5];
                break;
        }

        noRayInteractionButton.SetNativeSize();
        noRayInteractOption.text = text;


    }

    public void HideNoRayInteraction()
    {
        noRayInteractionHolder.gameObject.SetActive(false);
    }

    public void ShowNorayInteractionAlternate(string text, Keys key)
    {
        noRayInteractionAlternateHolder.gameObject.SetActive(true);

        switch (key)
        {
            case Keys.E:
                noRayAlternateInteractionButton.sprite = buttons[0];
                break;
            case Keys.F:
                noRayAlternateInteractionButton.sprite = buttons[1];
                break;
            case Keys.Q:
                noRayAlternateInteractionButton.sprite = buttons[4];
                break;
            case Keys.Space:
                noRayAlternateInteractionButton.sprite = buttons[2];
                break;
            case Keys.W:
                noRayAlternateInteractionButton.sprite = buttons[5];
                break;
        }

        noRayAlternateInteractionButton.SetNativeSize();
        noRayAlternateInteractOption.text = text;

    }

    public void HideAlternateRayInteraction()
    {
        //noRayInteractionAlternateHolder.gameObject.SetActive(false);
    }

    #endregion

    #region Functionality

    public void DisplayFadeTip(string tip)
    {
        tip = LocalizationSettings.StringDatabase.GetLocalizedString("uiManager", tip);
        fadeTipText.text = tip;
        fadeTipAnimation.Play();
    }

    public void DisplayMouse()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void LockMouse()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void ShowEndingPanel(Ending ending)
    {
        //HideNoRayInteraction();
        HideAlternateRayInteraction();
        HideCrosshair();
        HideInteractOption();
        HideAlternateRayInteraction();

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        endingPanel.ShowEndingPanel(ending);
    }

    public void HideCrosshair()
    {
        crosshair.enabled = false;
    }

    public void ShowCrosshair()
    {
        crosshair.enabled = true;
    }

    public void ShowCentralTip(string text)
    {
        centralTip.SetActive(true);

        //centralTipTextField.text = text;
        //centralTipTextField.enabled = true;
    }

    public void HideCentralTip()
    {
        centralTip.SetActive(false);

        centralTipTextField.text = string.Empty;
        centralTipTextField.enabled = false;
    }

    public void PauseOrUnpauseGame()
    {
        pausePanel.SetActive(!pausePanel.activeInHierarchy);

        if (!pausePanel.activeInHierarchy)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = pausePanel.activeInHierarchy;
        }

    }

    public void ShowHighlightTip()
    {
        highlightTipAnimation.Play();
    }

    public void PrintItemInspectInfo(string label, string description)
    {
        if (inspectFadeAnimation.isPlaying)
        {
            inspectFadeAnimation.Stop();
        }

        inspectDescriptionTextField.color = Color.white;
        inspectLabelTextField.color = Color.white;
        inspectButtonText.color = Color.white;
        inspectButtonLabel.color = Color.white;
        inspectBG.color = new Color32(0, 0, 0, 130);
        inspectButtonBorder.color = Color.white;

        inspectHolderBGAnimation.Play();

        inspectInformationHolder.SetActive(true);
        inspectLabelTextField.text = label;

        printMessageRoutine = StartCoroutine(PrintInspectRoutine(description, inspectDescriptionTextField));
    }

    IEnumerator PrintInspectRoutine(string text, TextMeshProUGUI textField)
    {
        int counter = 0;
        textField.text = string.Empty;
        inspectTypeSource.Play();

        while (textField.text.Length != text.Length)
        {
            textField.text += text[counter];
            counter++;

            yield return new WaitForSeconds(.02f);
        }

        inspectTypeSource.Stop();

    }

    public void HideInspectInfo()
    {
        //inspectInformationHolder.SetActive(false);
        //inspectLabelTextField.text = string.Empty;
        //inspectDescriptionTextField.text = string.Empty;

        if (inspectHolderBGAnimation.isPlaying) inspectHolderBGAnimation.Stop();
        inspectFadeAnimation.Play();
        inspectTypeSource.Stop();

        if(printMessageRoutine != null)
        {
            StopCoroutine(printMessageRoutine);
        }
       
    }

    public void PlaySound(AudioClip clip, float volume = 1)
    {
        if(clip == null) return;
        generalAudioSource.PlayOneShot(clip, volume);
    }

    #endregion

    #region button handlers

    public void OnReturnToMenuButtonClick()
    {
        Ending.ending.endingType = Ending.EndingType.Left;
        if (NetworkPlayerController.NetworkPlayer.isServer)
        {
            CustomNetworkManager.Instance.StopServer();
        }
        else
        {
            CustomNetworkManager.Instance.StopClient();
        }
    }

    public void OnQuitPauseButton()
    {
        Ending.ending = Ending.Left;
        if (NetworkPlayerController.NetworkPlayer.isServer)
        {
            //NetworkServer.Shutdown();
            CustomNetworkManager.Instance.StopHost();
        }
        else
        {
            CustomNetworkManager.Instance.StopClient();
        }


    }

    public void OnMouseSensSliderValueChanged(Slider slider)
    {
        NetworkPlayerController.NetworkPlayer._cameraController.mouseSensitivity = slider.value;
        PlayerAttributes.Sens = slider.value;
    }

    #endregion

}

