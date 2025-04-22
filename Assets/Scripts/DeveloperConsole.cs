using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class DeveloperConsole : MonoBehaviour
{
    PlayerControls consoleControls;
    public TMP_InputField consoleInput;
    public Canvas canvas;

    //temp
    public Shadow shadow;
    public Transform position;



    void Start(){
        consoleControls = new();
        consoleControls.Development.console.performed += OnConsole;
        consoleControls.Development.consoleEnter.performed += OnConsoleEnter;
        DontDestroyOnLoad(this.gameObject);
        consoleControls.Enable();
        consoleInput.onFocusSelectAll = true;
    }

    private void OnConsoleEnter(InputAction.CallbackContext context)
    {
        string command = consoleInput.text;

        if(command == "create shadow"){
            Instantiate(shadow, position.position, Quaternion.identity);
        }
        if(command == "start hunt"){
            GhostEvent.Instance.StartHuntEvent(false);
        }
        if(command == "screamer"){
            NetworkPlayerController.NetworkPlayer.Screamer();
        }

        consoleInput.text = string.Empty;
    }

    void OnConsole(InputAction.CallbackContext callback){
        canvas.gameObject.SetActive(!canvas.isActiveAndEnabled);
        if(canvas.isActiveAndEnabled){
            consoleInput.Select();
        }

        consoleInput.text = string.Empty;
        
    }

    
}
