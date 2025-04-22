using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Mirror;
using UnityEngine.Localization.Settings;

public class WifiApplication : MobileApplication
{
    [SerializeField] Router router;

    public bool isAuthorized;

    //components
    [SerializeField] Button connectButton;
    [SerializeField] GameObject authorizePanel;
    [SerializeField] GameObject wifiPanel;
    [SerializeField] TextMeshProUGUI wrongPasscodeText;
    [SerializeField] TMP_InputField inputField;
    [SerializeField] TextMeshProUGUI connectButtonTextField;

    //wifi icons
    [SerializeField] Image wifiIcon;
    [SerializeField] Sprite wifiOnIcon;
    [SerializeField] Sprite wifiOffIcon;

    private void Start()
    {
        connectButtonTextField.text = LocalizationSettings.StringDatabase.GetLocalizedString("inGameItems", "connect");
    }

    public void OnRouterOn()
    {
        wifiPanel.SetActive(true);
    }

    public void OnRouterOff()
    {
        DisconnectWifi();
    }

    public bool CheckPasswordValidation()
    {
        if(inputField.text == router.passcode)
        {
            return true;
        }
        else
        {
            return false;
        }    
    }

    public void DisconnectWifi()
    {
        wifiIcon.sprite = wifiOffIcon;
        connectButton.interactable = true;

        wifiPanel.SetActive(false);
        phone.hasWifi = false;
        connectButtonTextField.text = LocalizationSettings.StringDatabase.GetLocalizedString("inGameItems", "connect");

    }

    public void ConnectWifi()
    {
        wifiIcon.sprite = wifiOnIcon;
        connectButton.interactable = false;
        phone.hasWifi = true;
        connectButtonTextField.text = LocalizationSettings.StringDatabase.GetLocalizedString("inGameItems", "connected");

    }

    #region button events
    public void OnConnectClick()
    {
        if (isAuthorized)
        {
            ConnectWifi();
        }
        else
        {
            authorizePanel.SetActive(true);

            DisplayInternalTip();
        }
    }

    public void OnCancelClick()
    {
        inputField.text = string.Empty;
        authorizePanel.SetActive(false);
    }

    public void OnPasscodeSubmitClick()
    {
        if (CheckPasswordValidation())
        {
            CmdAuthorize();
        }
        else
        {
            wrongPasscodeText.enabled = true;
        }
    }

    #endregion

    [Command (requiresAuthority = false)]
    void CmdAuthorize()
    {
        RpcAuthorize();
    }
    [ClientRpc]
    void RpcAuthorize()
    {
        authorizePanel.SetActive(false);
        isAuthorized = true;
        ConnectWifi();
    }
}
