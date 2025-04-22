using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class UserDataCollector : MonoBehaviour
{
    private const string kReceiverEmailAddress = "nguserdata@gmail.com";

    private string _url = "https://docs.google.com/forms/u/2/d/e/1FAIpQLScxxOoL_BjV0_srsaTyb-OhEy0yNI3vyHckMHXLCTjJfWwWNA/formResponse";
    private  string _entryID = "entry.1533307544";

    bool _isOngoing;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void SendFeedback(string userFeedback, FeedbackType type)
    {
        if (_isOngoing) return;
        switch (type)
        {
            case FeedbackType.Feedback:
                _url = "https://docs.google.com/forms/u/2/d/e/1FAIpQLScxxOoL_BjV0_srsaTyb-OhEy0yNI3vyHckMHXLCTjJfWwWNA/formResponse";
                _entryID = "entry.1533307544";
                break;
            case FeedbackType.Error:
                _url = "https://docs.google.com/forms/u/2/d/e/1FAIpQLScRdRH6FmImCrqZfnqeW2___OMdv7POGSUMJatShTbd8xCdwg/formResponse";
                _entryID = "entry.1657362438";
                break;
            case FeedbackType.BugReport:
                _url = "https://docs.google.com/forms/u/2/d/e/1FAIpQLSfJvRrLa7Aj2LjumRBKBMxvBPqEAf1Jve-rdc-y5WoHTm0aYw/formResponse";
                _entryID = "entry.1276086849";
                break;
        }

        StartCoroutine(SendGFormData(userFeedback));
    }

    private IEnumerator SendGFormData<T>(T dataContainer)
    {
        _isOngoing = true;

        bool isString = dataContainer is string;
        string jsonData = isString ? dataContainer.ToString() : JsonUtility.ToJson(dataContainer);

        string fieldData = $"{DateTime.Today.ToString("D")}\n{SceneManager.GetActiveScene().name}\n\n{jsonData}\n===========================================";

        WWWForm form = new WWWForm();
        form.AddField(_entryID, fieldData);
        string urlGFormResponse = _url;
        using (UnityWebRequest www = UnityWebRequest.Post(urlGFormResponse, form))
        {
            yield return www.SendWebRequest();
        }

        _isOngoing = false;
    }

    // We cannot have spaces in links for iOS
    public static void OpenLink(string link)
    {
        bool googleSearch = link.Contains("google.com/search");
        string linkNoSpaces = link.Replace(" ", googleSearch ? "+" : "%20");
        Application.OpenURL(linkNoSpaces);
    }

    public enum FeedbackType
    {
        Error,
        Feedback,
        BugReport
    }
}