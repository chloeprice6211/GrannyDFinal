using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BugReportPanel : MonoBehaviour
{
    public TextMeshProUGUI bugReportTextField;
    public TMP_InputField inputField;
    [SerializeField] GameObject onSubmitPanel;

    [SerializeField] UserDataCollector dataCollector;

    [SerializeField] GameObject currentPanel;
    [SerializeField] GameObject menuPanel;

    [SerializeField] Button confirmButton;


    public void Submit()
    {
        string feedback = bugReportTextField.text;
        Debug.Log(feedback.Length);
        if (feedback.Length <=1) return;

        confirmButton.interactable = false;
        onSubmitPanel.SetActive(true);

        dataCollector.SendFeedback($"{feedback}", UserDataCollector.FeedbackType.BugReport);
    }

    public void ExitPanel()
    {
        confirmButton.interactable = true;
        inputField.text = string.Empty;

        onSubmitPanel.SetActive(false);
        currentPanel.SetActive(false);
        menuPanel.SetActive(true);
    }
}
