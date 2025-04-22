using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialMessage : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] Animation panelAnimation;

    [Header("UI components")]
    [SerializeField] TextMeshProUGUI messageTextField;
    [SerializeField] TextMeshProUGUI objectiveTextField;
    [SerializeField] Image BGImage;
    [SerializeField] Image buttonTipImage;
    [SerializeField] Animation bgAnimation;

    [SerializeField] List<Outline> interiorOutlineObjects;

    [Header("Objectives")]
    [SerializeField] TutorialKey tutorialKey;

    Coroutine printMessageRoutine;
    Coroutine objectiveMessageRoutine;

    bool _isCoroutineOngoing;
    //bool _isObjectiveCoroutineOngoing = false;

    string _currentText;
    string _currentObjective;

    PlayerControls _controls;
    public int count;
    string delayedtext;
    //int tempC = 0;

    [SerializeField] Fusebox fusebox;
    [SerializeField] TutorialScript script;

    private void Start()
    {
        _controls = new();
        _controls.Player.Tutorial.performed += TutorialPerformed;
        
        ShowTutorialMessage("Hey there! Welcome to The Escape: Together. Let's go through a small tutorial.");
    }

    private void TutorialPerformed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (_isCoroutineOngoing)
        {
            StopPrintingMessage();
        }
        else
        {
            HideTutorialMessage();
        }
    }

    public void ShowTutorialMessage(string message)
    {
        objectiveTextField.text = string.Empty;
        count++;
        _controls.Player.Tutorial.Enable();
        panelAnimation.Play("TutorialPanelAppear");
        _isCoroutineOngoing = true;
        messageTextField.text = string.Empty;

        printMessageRoutine = StartCoroutine(PrintMessageRoutine(message));
    }

    IEnumerator PrintMessageRoutine(string message)
    {
        int counter = 0;
        _currentText = message;

        yield return new WaitForSeconds(1f);

        audioSource.Play();

        while (messageTextField.text.Length != message.Length)
        {
            messageTextField.text += message[counter];
            counter++;
            yield return new WaitForSeconds(0.025f);
        }

        _isCoroutineOngoing = false;
        audioSource.Stop();
    }

    public void PrintObjective(string objective)
    {
        objectiveTextField.text = objective;
    }


    public void StopPrintingMessage()
    {
        _isCoroutineOngoing = false;
        StopCoroutine(printMessageRoutine);
        audioSource.Stop();
        messageTextField.text = _currentText;

        _currentText = string.Empty;
    }

    public void HideTutorialMessage()
    {
        if(count == 1)
        {
            UIManager.Instance.controlsMessageTip.ControlsMessage("go ahead and grab a flashlight");
        }
        else if(count == 2)
        {
            PrintObjective("objective: grab a note on the table");
        }
        else if(count == 3)
        {
            PrintObjective("objective: open journal by pressing [F1]");
            ShowKeyObjective();
        }
        else if(count == 4)
        {
            PrintObjective("objective: find a key");
        }
        else if(count == 5)
        {
            PrintObjective("objective: inspect the key by pressing [F]");
            
        }
        else if(count == 6)
        {
            PrintObjective("objective: open the door using the key");
           
        }
        else if(count == 8)
        {
            PrintObjective("objective: take the phone on the PC table");
           
        }
        else if(count == 9)
        {
            PrintObjective("objective: use a phone by pressing [R]");
        }
        else if(count == 10)
        {
            PrintObjective("objective: find the fusebox and restore electricity using your phone");
        }
        else if(count == 11)
        {
            ShowDelayed("You're not alone. Every house has it's own entity that will haunt you and will try to prevent your runaway by doing certain thing like closing doors, turning off power etc. You can use things you find in order to protect yourself as well as just being careful and use hideouts. You can learn more about basic items in the lobby menu.");
        }
        else if(count == 12)
        {
            ShowDelayed("And the last one. Teamwork is a key to success. Good luck.");
            PlayerPrefs.SetInt("tutorial", 1);
        }
        else if(count == 13)
        {
            script.EndTutorial();
        }

        _controls.Player.Tutorial.Disable();
        panelAnimation.Play("TutorialPanelDisappear");
        bgAnimation.Play();
    }

    public void ShowKeyObjective()
    {
        Invoke("DelayedKey", 5f);
       
    }

    public void ShowFuseboxObjective()
    {
        ShowTutorialMessage("House electricity is a crucial thing because it gives you a lot of ways of interactions." +
            " For instance, when the lights are on, you have access to Wi-Fi, meaning you can search for information in the phone apps that require connection.");
    }

    public void ShowPhoneObjective()
    {
        Invoke("DelayedFusebox", 2f);
    }

    public void ShowJournalObjective()
    {
        ShowTutorialMessage("You can access found notes in your journal. To open it, press [F1].");
    }


    void DelayedFusebox()
    {
        fusebox.PerformGhostInteraction();
       
        Invoke("DelayedPhone", 1f);
    }
    void DelayedKey()
    {
        ShowTutorialMessage("Most of environment stuff can be interacted with. You can open drawers, doors, turn on electronic devices etc." +
           " Now, grab a key in the closet.");

        foreach (Outline outline in interiorOutlineObjects)
        {
            outline.enabled = true;
        }

        tutorialKey.gameObject.layer = 7;
        tutorialKey.outlineShader.enabled = true;
    }

    void DelayedPhone()
    {
        ShowTutorialMessage("Oh no! The lights are off. Find a fusebox, and restore the electricity using your" +
          " phone.");
    }

    void ShowDelayed(string text)
    {
        delayedtext = text;
        Invoke("DelayedMessage", 1f);
    }

    void DelayedMessage()
    {
        ShowTutorialMessage(delayedtext);
    }

}
