using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WifiDesktopApplication : DesktopApplication
{
    [SerializeField] Router router;
    [SerializeField] GameObject networkHolder;
    [SerializeField] GameObject passcodeInputHolder;
    [SerializeField] TMP_InputField inputField;
    [SerializeField] TextMeshProUGUI connectButtonText;
    [SerializeField] Button connectButton;
    [SerializeField] Button wifiButton;
    [SerializeField] Sprite onWifi;
    [SerializeField] Sprite offWifi;
    public bool isAuthorized;

    public void OnConnectClick()
    {
        passcodeInputHolder.SetActive(true);

        if (isAuthorized)
        {
            Connect();
        }
    }

    public void OnApplyClick()
    {
        if(inputField.text == router.passcode)
        {
            Connect();
        }
    }

    void Connect()
    {
        connectButtonText.text = "CONNECTED";
        connectButton.enabled = false;
        passcodeInputHolder.SetActive(false);
        newLaptop.hasWifi = true;
        isAuthorized = true;
        wifiButton.image.sprite = onWifi;

    }

    public void OnBackClick()
    {
        passcodeInputHolder.SetActive(false);
    }

    public void OnRouterOn()
    {
        networkHolder.SetActive(true);
    }

    public void OnRouterOff()
    {
        networkHolder.SetActive(false);
        newLaptop.hasWifi = false;
        wifiButton.image.sprite = offWifi;

        connectButtonText.text = "CONNECT";
        connectButton.enabled = true;
    }
}
