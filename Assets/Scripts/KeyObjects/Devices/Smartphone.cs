using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
using Mirror;

public class Smartphone : Device, IGenerable
{
    [SerializeField] Animation lockScreenAttemp;
    [SerializeField] TextMeshProUGUI codeTextField;
    [SerializeField] TextMeshProUGUI combinationText;

    [SerializeField] GameObject lockScreenPanel;
    [SerializeField] GameObject mainScreenPanel;
    [SerializeField] GameObject wifiPanel;
    [SerializeField] GameObject lightManagerPanel;
    [SerializeField] GameObject messangerPanel;
    [SerializeField] Canvas _canvas;

    [SerializeField] AudioClip homeButtonPressClip;
    public bool isActive = false;

    private string _combination;
    [SyncVar] public string unlockCombination;
    public bool hasWifi;


    public MobileApplication currentApplication = null;


    public override void UseDevice(InputAction.CallbackContext context)
    {
        if (!_owner.hasAuthority) return;

        if (!isActive)
        {
            CmdUnlockScreen();
        }

        base.UseDevice(context);
    }

    //functionality
    public void AddCombinationDigit(int digit)
    {
        _combination += digit;
        combinationText.text += "*";

        if (_combination.Length == 4)
        {
            CheckCombinationValidality();
        }
    }

    private void CheckCombinationValidality()
    {
        if (_combination == unlockCombination)
        {
            CmdUnlockPhone();
        }
        else
        {
            _combination = string.Empty;
            combinationText.text = string.Empty;

            lockScreenAttemp.Play();
        }
    }

    //button listeners
    public void OnHomeButtonClick()
    {
        AudioSource.PlayClipAtPoint(homeButtonPressClip, transform.position, .5f);

        if (currentApplication != null)
        {
            currentApplication.CloseApplication();
        }
    }

    public void ApplyGeneratedCode(string code)
    {
        unlockCombination = code;
    }
    public void ShowGeneratedCode()
    {
        codeTextField.text = unlockCombination;
    }

    [Command (requiresAuthority = false)]
    void CmdUnlockScreen()
    {
        RpcUnlockScreen();
    }
    [ClientRpc]
    void RpcUnlockScreen()
    {
        _canvas.enabled = true;
        isActive = true;
    }

    [Command (requiresAuthority = false)]
    void CmdUnlockPhone()
    {
        RpcUnlockPhone();
    }
    [ClientRpc]
    void RpcUnlockPhone()
    {
        lockScreenPanel.SetActive(false);
        mainScreenPanel.SetActive(true);
    }
}
