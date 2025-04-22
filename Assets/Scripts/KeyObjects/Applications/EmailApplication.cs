using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Mirror;
using UnityEngine.Rendering;
using UnityEngine.Localization.Settings;
using Unity.VisualScripting;

public class EmailApplication : MobileApplication, IGenerable, IReceivePassword, IReceiveMessage
{
    //screens
    [SerializeField] GameObject inboxScreen;
    [SerializeField] GameObject emailLetterScreen;
    [SerializeField] GameObject noConnectionScreen;
    [SerializeField] GameObject signInScreen;

    //email letter screen
    [SerializeField] TextMeshProUGUI senderNameText;
    [SerializeField] TextMeshProUGUI emailDateText;
    [SerializeField] TextMeshProUGUI emailContentText;
    [SerializeField] Transform emailLetters;

    [SerializeField] TextMeshProUGUI codeTextField;
    [SerializeField] GameObject receivedLetter;

    //login
    [SerializeField] TMP_InputField loginInputField;
    [SerializeField] TMP_InputField passwordInputField;
    [SerializeField] TextMeshProUGUI wrongText;
    
    public string email;
    [SyncVar] public string password;

    [SerializeField] EmailLetter receiveLetter;


    public override void LaunchApplication()
    {
        base.LaunchApplication();

        if (!phone.hasWifi && noConnectionScreen != null)
        {
            noConnectionScreen.SetActive(true);
        }
    }

    private void SetEmailLetterScreen(EmailLetter eLetter)
    {
        senderNameText.text = eLetter.senderName;

        Debug.Log(eLetter.textContent);

        emailContentText.text = LocalizationSettings.StringDatabase.GetLocalizedString("inGameItems" , eLetter.textContent);
        if (!string.IsNullOrEmpty(eLetter.combination))
            emailContentText.text += eLetter.combination;

        emailDateText.text = eLetter.date;
    }

    public void OnInternetConnectionLost()
    {
        noConnectionScreen.SetActive(true);
    }

    public void ApplyGeneratedCode(string code)
    {
        password = code;
    }
    public void ShowGeneratedCode()
    {
        codeTextField.text = password;
    }

    #region client server

    [Command (requiresAuthority = false)]
    void CmdAuthorize()
    {
        RpcAuthorize();
    }
    [ClientRpc]
    void RpcAuthorize()
    {
        signInScreen.SetActive(false);
        inboxScreen.SetActive(true);
    }

    #endregion

    #region button handlers

    public void OnBackClick()
    {
        emailLetterScreen.SetActive(false);
        inboxScreen.SetActive(true);
    }

    public void OnRetryClick()
    {
        if (phone.hasWifi)
        {
            noConnectionScreen.SetActive(false);
        }
    }

    public void OnSignInClick()
    {
        if (passwordInputField.text == password)
        {
            CmdAuthorize();
            DisplayInternalTip();
        }
        else
        {
            wrongText.enabled = true;
        }
    }

    public void OnLetterClick(Button button)
    {
        EmailLetter _eLetter = button.transform.parent.GetComponent<EmailLetter>();

        inboxScreen.SetActive(false);

        SetEmailLetterScreen(_eLetter);
        emailLetterScreen.SetActive(true);

    }

    public void Receive()
    {
        emailLetters.transform.localPosition = new Vector3(0, -1, 0);
        receivedLetter.SetActive(true);
    }

    public void DisplayPassword(string code)
    {
        receiveLetter.combination += code;
    }

    #endregion
}
