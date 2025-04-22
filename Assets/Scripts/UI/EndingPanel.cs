using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text;
using UnityEngine.UI;
using System;
using UnityEngine.Localization.Settings;

public class EndingPanel : MonoBehaviour
{
    [SerializeField] UserDataCollector dataCollector;
    [SerializeField] TextMeshProUGUI conclusionText;
    [SerializeField] List<RatingStar> stars;
    public Animation anim;
    public TextMeshProUGUI feedbackText;
    string _result;

    [SerializeField] Button button;

    public int _currentRating = 999;

    public string feedback;

    List<string> texts = new();


    public void ShowEndingPanel(Ending result)
    {
        StartCoroutine(EndingPanelRoutine(result));
    }

    IEnumerator EndingPanelRoutine(Ending result)
    {
        GenerateText(conclusionText, result, .07f);
        yield return null;
    }

    public void GenerateText(TextMeshProUGUI textField, Ending result, float rate)
    {
        StringBuilder stringB = new(LocalizationSettings.StringDatabase.GetLocalizedString("mainMenu",result.conclusionMessage));
        _result = result.endingType.ToString();
        StartCoroutine(GenerateTextRoutine(textField, stringB, rate));
    }

    IEnumerator GenerateTextRoutine(TextMeshProUGUI textField, StringBuilder text, float rate)
    {
        int charCounter = 0;

        while (charCounter < text.Length)
        {
            textField.text += text[charCounter];

            charCounter++;
            yield return new WaitForSeconds(rate);
        }

        yield return null;
    }

    public void StarClick(int index)
    {
        if (!button.interactable) button.interactable = true;

        for (int a = 0; a < 5; a++)
        {
            if (a > index)
            {
                stars[a].Disable();
            }
            else
            {
                stars[a].Enable();
            }
        }

        _currentRating = index;
    }

    public void OnSubmit()
    {
        if (_currentRating == 999) return;

        anim.Play();
        button.interactable = false;
        string feedback = $"result is {_result}, rating is {_currentRating + 1}, {feedbackText.text}";

        dataCollector.SendFeedback(feedback, UserDataCollector.FeedbackType.Feedback);
    }
}
