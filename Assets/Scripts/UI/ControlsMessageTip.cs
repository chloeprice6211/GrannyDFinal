using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Localization.Settings;

public class ControlsMessageTip : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI controlsMessageTextField;
    private List<string> controlsMessagesQueue = new();

    [SerializeField] Animation tipBGAnimation;

    bool _isControlsMessageRoutineOngoing;

    public void ControlsMessage(string message)
    {
        message = LocalizationSettings.StringDatabase.GetLocalizedString("characterUiMessage", message);

        controlsMessagesQueue.Add(message);

        if (controlsMessagesQueue.Count > 1)
        {
            if (message == controlsMessagesQueue[controlsMessagesQueue.Count - 2])
            {
                controlsMessagesQueue.RemoveAt(controlsMessagesQueue.Count - 1);
            }
        }

        if (_isControlsMessageRoutineOngoing)
        {
            return;
        }

        StartCoroutine(ControlsMessageCoroitone(controlsMessagesQueue[0]));

    }

    private IEnumerator ControlsMessageCoroitone(string message)
    {
        string goingText = string.Empty;
        int charIndex = 0;

        _isControlsMessageRoutineOngoing = true;

        tipBGAnimation.Play();

        yield return new WaitForSeconds(1f);

        while (charIndex != message.Length)
        {
            controlsMessageTextField.text += message[charIndex];
            charIndex++;

            yield return new WaitForSeconds(.02f);
        }

        yield return new WaitForSeconds(controlsMessageTextField.text.Length / 20 + 1.5f);
        StartCoroutine(EraseControlsMessageCoroutine());

    }

    private IEnumerator EraseControlsMessageCoroutine()
    {
        while (controlsMessageTextField.text.Length != 0)
        {
            controlsMessageTextField.text = controlsMessageTextField.text.Remove(controlsMessageTextField.text.Length - 1);

            yield return new WaitForSeconds(.01f);
        }
        
        Invoke("ShiftControlsMessageQueueArray", 1f);
    }

    private void ShiftControlsMessageQueueArray()
    {
        _isControlsMessageRoutineOngoing = false;
        tipBGAnimation.Stop();
        if (controlsMessagesQueue.Count > 1)
        {
            for (int a = 0; a < controlsMessagesQueue.Count - 1; a++)
            {
                controlsMessagesQueue[a] = controlsMessagesQueue[a + 1];
            }

            controlsMessagesQueue.RemoveAt(controlsMessagesQueue.Count - 1);
            StartCoroutine(ControlsMessageCoroitone(controlsMessagesQueue[0]));
        }
        else
        {
            controlsMessagesQueue.RemoveAt(controlsMessagesQueue.Count - 1);
        }
    }

}
