using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OperatingSystem : NetworkBehaviour, IGenerable
{
    [Header("welcome screen components")]
    [SerializeField] Button enterButton;
    [SerializeField] Button forgotPasswordButton;
    [SerializeField] GameObject welcomeScreenPanel;
    [SerializeField] TextMeshProUGUI wrongPasscodeTextField;

    [SerializeField] TMP_InputField welcomeScreenPasscodeInputField;
    public bool hasPasscode;
    [SyncVar] public string welcomeScreenPasscode;

    [SerializeField] TextMeshProUGUI resetPasscodeLabel;
    [SerializeField] EmailLetter letterToReceive;

    [SerializeField] GameObject messageReceiveObject;
    IReceiveMessage _messageReceiver;

    [Header("main screen components")]
    [SerializeField] GameObject mainScreenPanel;

    [Header("OS")]
    public DesktopApplication currentApplication;

    [Header("Audio")]
    [SerializeField] List<AudioClip> keyboardButtonClips; 

    public void Enter()
    {
        if (hasPasscode)
        {
            if(welcomeScreenPasscode == welcomeScreenPasscodeInputField.text)
            {
                EnterCmd();
            }
            else
            {
                wrongPasscodeTextField.enabled = true;
                welcomeScreenPasscodeInputField.text = string.Empty;
            }
        }
        else
        {
            welcomeScreenPanel.SetActive(false);
            mainScreenPanel.SetActive(true);
            mainScreenPanel.transform.localScale = new Vector3(1, 1, 1);
        }
    }

    [Command (requiresAuthority = false)]
    void EnterCmd()
    {
        EnterRpc();
    }

    [ClientRpc]
    void EnterRpc()
    {
        welcomeScreenPasscodeInputField.text = string.Empty;
        welcomeScreenPanel.SetActive(false);
        mainScreenPanel.gameObject.SetActive(true);
        mainScreenPanel.transform.localScale = new Vector3(1, 1, 1);
    }

    public void OnForgotPasscode()
    {
        ForgotCmd();
    }

    [Command (requiresAuthority = false)]
    void ForgotCmd()
    {
        ForgotRpc();
    }

    [ClientRpc]
    void ForgotRpc()
    {
        resetPasscodeLabel.enabled = true;
        resetPasscodeLabel.gameObject.GetComponent<Animation>().Play();

        if (messageReceiveObject.TryGetComponent(out _messageReceiver))
        {
            _messageReceiver.Receive();
        }
    }

    public void ApplyGeneratedCode(string code)
    {
        welcomeScreenPasscode = code;
    }

    public void ShowGeneratedCode()
    {
        letterToReceive.combination += welcomeScreenPasscode;
    }

    public void OnKeyboard()
    {
        int clipIndex = Random.Range(0, keyboardButtonClips.Count);
        AudioSource.PlayClipAtPoint(keyboardButtonClips[clipIndex], transform.position);
    }
}
