using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PausePanel : MonoBehaviour
{
    [SerializeField] GameObject quitConfimationPanel;
    [SerializeField] GameObject settingsPanel;
    [SerializeField] GameObject controlsPanel;

    #region event handlers

    public void OnReturnFromControslClick()
    {
        controlsPanel.SetActive(false);
    }

    public void OnReturnFromSettingsClick()
    {
        settingsPanel.SetActive(false);
    }

    public void OnSettingsClick()
    {
        settingsPanel.SetActive(true);
    }

    public void OnControlsClick()
    {
        controlsPanel.SetActive(true);
    }

    public void OnContinueClick()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        gameObject.SetActive(false);
    }
    public void OnExitClick()
    {
        quitConfimationPanel.SetActive(true);
    }

    public void OnExitDecline()
    {
        quitConfimationPanel.SetActive(false);
    }

    public void OnExitConfirm()
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

    public void OnTutorialExitConfirm()
    {
        PlayerPrefs.SetInt("tutorial", 1);
        if (NetworkPlayerController.NetworkPlayer.isServer)
        {
            //NetworkServer.Shutdown();
            CustomNetworkManager.Instance.StopHost();
        }
        else
        {
            CustomNetworkManager.Instance.StopClient();
        }
        Destroy(CustomNetworkManager.Instance.gameObject);
        
    }
    #endregion
}
