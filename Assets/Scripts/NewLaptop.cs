using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;
using UnityEngine.UI;
using TMPro;

public class NewLaptop : Device, IGenerable, IInsertable
{
    [SerializeField] Transform osDiscPosition;

    [Header("UI")]
    [SerializeField] GameObject welcomeScreen;
    [SerializeField] GameObject mainScreen;
    [SerializeField] GameObject OSInstallScreen;
    [SerializeField] GameObject OSLoadingScreen;

    [SerializeField] Animation screenAnimation;
    [SerializeField] Animation CDInsertAnimation;
    [SerializeField] Light lightSource;

    [SerializeField] AudioClip laptopOpening;
    [SerializeField] AudioClip laptopClosing;

    [SerializeField] TextMeshProUGUI consoleTextField;

    [Header("OS")]
    [SerializeField] GameObject console;
    [SerializeField] Slider osInstallSlider;
    [SerializeField] Button installButton;
    [SerializeField] Button consoleApplyButton;
    [SerializeField] GameObject consoleInstall;
    [SerializeField] Slider consoleInstallSlider;
    [SerializeField] TextMeshProUGUI insetedCommand;
    [SerializeField] TextMeshProUGUI wrongCommandText;
    [SerializeField] TMP_InputField consoleInputField;
    [SerializeField] GameObject rebootPanel;
    [SyncVar] public string installDriversCommand;
    [SerializeField] GameObject loadingPanel;

    [SerializeField] AudioClip cdInsert;

    public bool hasWifi;
    public bool isOn;
    public bool hasCD;
    public DesktopApplication currentApplication;

    private void Start()
    {
        OSInstallScreen.SetActive(false);
        OSLoadingScreen.SetActive(false);
        welcomeScreen.SetActive(false);
        mainScreen.SetActive(false);
    }

    //functionality
    public void TurnDisplayOn()
    {
        AudioSource.PlayClipAtPoint(laptopOpening, transform.position, .5f);
        isOn = true;
        canvas.enabled = true;
        lightSource.enabled = true;

    }

    public void TurnDisplayOff()
    {
        AudioSource.PlayClipAtPoint(laptopClosing, transform.position, .5f);
        isOn = false;
        canvas.enabled = false;
        lightSource.enabled = false;

    }


    //input system
    public override void ExitDevice(InputAction.CallbackContext obj)
    {
        if (screenAnimation.isPlaying) return;

        if (!_owner.hasAuthority) return;

        CmdExitDevice();
        base.ExitDevice(obj);

    }

    public override void UseDevice(InputAction.CallbackContext obj)
    {
        if (screenAnimation.isPlaying) return;

        if (!_owner.hasAuthority) return;

        CmdUseDevice();
        base.UseDevice(obj);

    }



    #region remote override actions

    [Command(requiresAuthority = false)]
    private void CmdUseDevice()
    {
        RpcUseDevice();
    }
    [ClientRpc]
    private void RpcUseDevice()
    {
        TurnDisplayOn();
        screenAnimation.Play("LaptopScreenUp");
    }

    [Command(requiresAuthority = false)]
    private void CmdExitDevice()
    {
        RpcExitDevice();
    }
    [ClientRpc]
    private void RpcExitDevice()
    {
        Invoke("TurnDisplayOff", .6f);
        screenAnimation.Play("LaptopScreenDown");
    }

    #endregion

    #region interface implementation

    public override void OnDropItem()
    {
        base.OnDropItem();
        gameObject.GetComponent<Collider>().enabled = true;
    }

    #endregion

    //button listeners
    public void OnEnterClick()
    {
        CmdEnterSystem();
    }

    #region OS insert

    [Command(requiresAuthority = false)]
    private void InsertCDCommand(OSDisc disc)
    {
        InsertCDRpc(disc);
    }
    [ClientRpc]
    private void InsertCDRpc(OSDisc disc)
    {
        hasCD = true;

        Transform parentItem = disc.transform.parent;

        parentItem.SetParent(osDiscPosition);
        parentItem.localPosition = Vector3.zero;
        parentItem.localRotation = Quaternion.Euler(Vector3.zero);

        OSInstallScreen.SetActive(true);

        CDInsertAnimation.Play();
    }

    #endregion

    #region enter system remote action

    //UI management
    public void EnterSystem()
    {
        welcomeScreen.SetActive(false);
        mainScreen.SetActive(true);
    }

    [Command(requiresAuthority = false)]
    private void CmdEnterSystem()
    {
        RpcEnterSystem();
    }
    [ClientRpc]
    private void RpcEnterSystem()
    {
        EnterSystem();
    }

    #endregion

    public void InstallOS()
    {
        CmdInstallOS();
    }

    [Command(requiresAuthority = false)]
    void CmdInstallOS()
    {
        RpcInstallOS();
    }
    [ClientRpc]
    void RpcInstallOS()
    {
        StartCoroutine(OSInstallRoutine());
    }
    IEnumerator OSInstallRoutine()
    {
        osInstallSlider.gameObject.SetActive(true);
        installButton.gameObject.SetActive(false);

        while (osInstallSlider.value != 100)
        {
            yield return new WaitForSeconds(1f);
            osInstallSlider.value += 2;
        }

        ShowConsolePrompt();
        Debug.Log("install completed");
    }

    public void InstallDrivers()
    {
        if (consoleInputField.text == installDriversCommand)
        {
            DriversCmd();
        }
        else
        {
            wrongCommandText.enabled = true;
        }

        consoleInputField.text = "";

    }

    [Command (requiresAuthority = false)]
    void DriversCmd()
    {
        DriversRpc();
    }
    [ClientRpc]
    void DriversRpc()
    {
        insetedCommand.text = @">> C:\OS\Users> " + installDriversCommand;
        consoleInstall.SetActive(true);
        StartCoroutine(DriverInstallRoutine());
    }

    IEnumerator DriverInstallRoutine()
    {
        while (consoleInstallSlider.value != 100)
        {
            yield return new WaitForSeconds(1f);
            consoleInstallSlider.value += 5;
        }

        rebootPanel.SetActive(true);
    }

    public void Reboot()
    {
        RebootCmd();
    }

    [Command (requiresAuthority = false)]
    void RebootCmd()
    {
        RebootRpc();
    }
    [ClientRpc]
    void RebootRpc()
    {
        StartCoroutine(RebootRoutine());
    }

    IEnumerator RebootRoutine()
    {
        OSInstallScreen.SetActive(false);
        canvas.enabled = false;
        yield return new WaitForSeconds(5);

        OSLoadingScreen.SetActive(true);
        canvas.enabled = true;

        yield return new WaitForSeconds(10);

        OSLoadingScreen.SetActive(false);
        welcomeScreen.SetActive(true);

    }

    void ShowConsolePrompt()
    {
        //UIManager.Instance.Message("Well.. I could use a Computer Science book. Must be nearby.");
        console.GetComponent<Animation>().Play();
    }

    public void SignIn()
    {
        SignCmd();
    }
    [Command (requiresAuthority = false)]
    void SignCmd()
    {
        SignRpc();
    }
    [ClientRpc]
    void SignRpc()
    {
        welcomeScreen.SetActive(false);
        mainScreen.SetActive(true);
    }

    public void ApplyGeneratedCode(string code)
    {
        installDriversCommand = "sfc /scannow -" + code;
    }

    public void ShowGeneratedCode()
    {
        consoleTextField.text = installDriversCommand;
    }

    public void Insert(UtilityItem itemToInsert, NetworkPlayerController owner)
    {
        InsertCmd(itemToInsert);
    }

    [Command (requiresAuthority = false)]
    void InsertCmd(UtilityItem itemToInsert)
    {
        InsertRpc(itemToInsert);
    }
    [ClientRpc]
    void InsertRpc(UtilityItem item)
    {
        hasCD = true;

        Transform parentItem = item.transform.parent;

        parentItem.SetParent(osDiscPosition);
        parentItem.localPosition = Vector3.zero;
        parentItem.localRotation = Quaternion.Euler(Vector3.zero);

        UIManager.Instance.PlaySound(cdInsert);
        item.GetComponent<Collider>().enabled = false;

        parentItem.localScale = item.parentScale;

        OSInstallScreen.SetActive(true);

        CDInsertAnimation.Play();
    }
}
