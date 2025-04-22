using System.Collections;
using System.Collections.Generic;
using TMPro;

using UnityEngine;

public class MessagePrint: MonoBehaviour
{
    bool _stopAudioSource;

    public bool _isCoroutineOngoing
    {
        get; private set;
    }

    Coroutine printMessageRoutine;

    static MessagePrint _instance;

    public static MessagePrint Instance
    {
        get { return _instance; }
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        _instance = this;
    }

    public void Message(string message, TextMeshProUGUI textField, AudioClip audioClip = null, float rate = 0.025f, bool displaySound = false)
    {
        textField.text = string.Empty;
        _isCoroutineOngoing = true;

        printMessageRoutine = StartCoroutine(PrintMessageRoutine(message, textField, audioClip, rate, displaySound));
    }

    IEnumerator PrintMessageRoutine(string message, TextMeshProUGUI textField, AudioClip clip, float rate, bool displaySound)
    {
        int counter = 0;

        //yield return new WaitForSeconds(1f);

        //UIManager.Instance.au

        _stopAudioSource = displaySound;

        if (displaySound)
            UIManager.Instance.inspectTypeSource.Play();

        while (textField.text.Length != message.Length)
        {
            textField.text += message[counter];
            counter++;
            yield return new WaitForSeconds(rate);
        }

        if (displaySound)
            UIManager.Instance.inspectTypeSource.Stop();

        _isCoroutineOngoing = false;
        //audioSource.Stop();
    }

    public void StopPrinting()
    {
        if (printMessageRoutine != null)
        {
            StopCoroutine(printMessageRoutine);

            if (UIManager.Instance != null && _stopAudioSource)
                UIManager.Instance.inspectTypeSource.Stop();
        }
           

        _isCoroutineOngoing = false;
    }
}
