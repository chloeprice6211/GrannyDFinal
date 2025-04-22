using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MessangerDesktopApplication : DesktopApplication, IGenerable, IReceivePassword
{
    [SerializeField] Image imageToDisplay;
    [SerializeField] Smartphone phone;
    [SerializeField] TMP_InputField codeInputField;
    [SerializeField] GameObject authPanel;
    [SerializeField] GameObject mainPanel;
    [SerializeField] GameObject textFields;
    [SerializeField] MessangerApplication app;
    [SerializeField] GameObject codeSent;
    [SerializeField] GameObject noConnectionPanel;
    [SerializeField] TextMeshProUGUI passwordField;

    public List<GameObject> panels;

    [SerializeField] int currentMessangerObjectIndex = 0;

    [SyncVar] public string generatedCode = "5555";

    public void OnMessageClick(int index)
    {
        SetGameObject(index);
    }
    public void SetGameObject(int index)
    {
        panels[currentMessangerObjectIndex].SetActive(false);
        panels[index].SetActive(true);

        currentMessangerObjectIndex = index;
    }

    public void OnSendCodeClick()
    {
        CmdSendCode();
    }

    [Command (requiresAuthority = false)]
    void CmdSendCode()
    {
        RpcSendCode();
    }
    [ClientRpc]
    void RpcSendCode()
    {
        app.ShowMessage();
        codeSent.SetActive(true);
    }

    public void OnApplyCode()
    {
        if(codeInputField.text == generatedCode)
        {
            ApplyCmd();
        }
    }

    [Command (requiresAuthority = false)]
    void ApplyCmd()
    {
        ApplyRpc();
    }
    [ClientRpc]
    void ApplyRpc()
    {
        authPanel.SetActive(false);
        mainPanel.SetActive(true);
    }

    public void ApplyGeneratedCode(string code)
    {
        generatedCode = code;
    }

    public void ShowGeneratedCode()
    {
        app.SetMessage(generatedCode);
    }

    public void OnRetryClick()
    {
        if (newLaptop.hasWifi)
        {
            CmdRetry();
        }
    }

    [Command (requiresAuthority = false)]
    void CmdRetry()
    {
        RpcRetry();
    }
    [ClientRpc]
    void RpcRetry()
    {
        noConnectionPanel.SetActive(false);
    }

    public void OnRouterOff()
    {
        noConnectionPanel.SetActive(true);
    }

    public void DisplayPassword(string code)
    {
        passwordField.text = code;
    }
}
