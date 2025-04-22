using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Mirror;
using UnityEngine.Localization.Settings;
using UnityEngine.Rendering.PostProcessing;

public class DeathPanel : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI mainMessageTextField;
    [SerializeField] GameObject spectatePanel;
    [TextArea]
    public string messageToDisplay;

    [TextArea]
    public string reviveMessageToDisplay;

    [SerializeField] GameObject gridHolder;

    public Button returnButton;
    public Button spectateButton;

    public int currentPlayerIndex;

    PlayerControls _controls;

    private void Awake()
    {
        _controls = new();
        _controls.Disable();
    }

    public void ShowEndingPanel(Ending ending)
    {
        gameObject.SetActive(true);
        messageToDisplay = LocalizationSettings.StringDatabase.GetLocalizedString("mainMenu", ending.message);

        StartCoroutine(PrintMessageRoutine(ending));
    }

    private IEnumerator PrintMessageRoutine(Ending ending)
    {
        int counter = 0;

        mainMessageTextField.text = "";

        while (counter != messageToDisplay.Length)
        {
            mainMessageTextField.text += messageToDisplay[counter];
            counter++;

            yield return new WaitForSeconds(.05f);
        }
        
        if(GameManager.Instance.playersAlive > 0 && ending.endingType != Ending.EndingType.Escaped)
        {
            ShowGridButtons(GameManager.Instance.playersAlive > 0);
        }
        else
        {
            GameManager.Instance.CmdEndGame();
        }

        

    }

    void ShowGridButtons(bool isSpectateEnabled)
    {
        if (isSpectateEnabled)
        {
            spectateButton.gameObject.SetActive(true);
        }
        returnButton.gameObject.SetActive(true);
    }

    public void ShowMainCamera()
    {
        foreach(NetworkPlayerController controller in CustomNetworkManager.Instance.GamePlayers)
        {
            if (controller.hasAuthority)
            {
                controller.playerCamera.gameObject.SetActive(true);

                controller._cameraController.virtualCamera.gameObject.SetActive(true);
                controller.body.transform.localScale = Vector3.zero;
            }
            else
            {
                controller.playerCamera.gameObject.SetActive(false);

                controller._cameraController.virtualCamera.gameObject.SetActive(false);
                controller.body.transform.localScale = Vector3.one;
            }
        }

        _controls.Spectate.Next.Disable();
        spectatePanel.gameObject.SetActive(false);
    }

    #region button handlers

    public void OnSpectateClick()
    {
        _controls.Spectate.Next.performed += NextPerformed; ;
        _controls.Spectate.Next.Enable();
        
        NetworkPlayerController.NetworkPlayer.RemoveDeathEffect();
        NetworkPlayerController.NetworkPlayer.SwitchCamera();

        NetworkPlayerController.NetworkPlayer.body.transform.localScale = new Vector3(1, 1, 1);

        NetworkPlayerController buddy = CustomNetworkManager.Instance.GamePlayers[0].hasAuthority ?
            CustomNetworkManager.Instance.GamePlayers[1] :
            CustomNetworkManager.Instance.GamePlayers[0];

        currentPlayerIndex = CustomNetworkManager.Instance.GamePlayers.IndexOf(buddy);

        NetworkPlayerController.NetworkPlayer._cameraController._grain.intensity.value = 0;
        NetworkPlayerController.NetworkPlayer._cameraController._grain.size.value = 0;

        buddy.body.transform.localScale = Vector3.zero;
        gameObject.SetActive(false);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        

        spectatePanel.SetActive(true);
    }


    private void NextPerformed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        Debug.Log("performed");

        if (currentPlayerIndex + 1 > CustomNetworkManager.Instance.GamePlayers.Count - 1)
        {
            currentPlayerIndex = 0;
        }
        else
        {
            currentPlayerIndex++;
        }

        foreach (NetworkPlayerController player in CustomNetworkManager.Instance.GamePlayers)
        {
            player.body.transform.localScale = new Vector3(1, 1, 1);
            player._cameraController.virtualCamera.gameObject.SetActive(false);
        }

        CustomNetworkManager.Instance.GamePlayers[currentPlayerIndex].body.transform.localScale = Vector3.zero;
        CustomNetworkManager.Instance.GamePlayers[currentPlayerIndex]._cameraController.virtualCamera.gameObject.SetActive(true);
    }

    public void OnReturnClick()
    {
        Ending.ending = NetworkPlayerController.NetworkPlayer.isAlive ? Ending.Escaped : Ending.Death;

        if (NetworkPlayerController.NetworkPlayer.isServer)
        {
            CustomNetworkManager.Instance.StopHost();
        }
        else
        {
            CustomNetworkManager.Instance.StopClient();
        }
    }

    private void OnDisable()
    {
        //
    }
    private void OnDestroy()
    {
        _controls.Disable();
    }

    #endregion

}
