using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public struct SystemMessageColor
{
    public static string Orange
    {
        get
        {
            return "#ff6600";
        }
    }
    public static string Green
    {
        get
        {
            return "#2fedbb";
        }
    }
    public static string Red
    {
        get
        {
            return "#d61546";
        }
    }
    public static string White
    {
        get
        {
            return "#ffffff";
        }
    }
}

public class Logger : MonoBehaviour
{
    public static Logger instance { get; private set; }

    [SerializeField] GameObject chatMessagePrefab;
    [SerializeField] GameObject scrollContent;
    [SerializeField] Scrollbar scrollBar;

    private string playerMessageColorHex = "#25dbb1";

    public TMP_InputField inputField;
    public string InputFieldContent
    {
        get
        {
            return inputField.text;
        }
        set
        {
            inputField.text = value;
        }
    }


    private void Awake()
    {
        instance = this;
    }

    public void LogSystemMessage(string message, string messageColor)
    {
        chatMessagePrefab.GetComponent<TextMeshProUGUI>().text = $"[{System.DateTime.Now.ToString("HH:mm")}] <color={messageColor}>{message}</color>";
        GameObject obj = Instantiate(chatMessagePrefab, scrollContent.transform);

    }

    public void LogPlayerMessage(string playerName, string message)
    {
        chatMessagePrefab.GetComponent<TextMeshProUGUI>().text = $"<b><color={playerMessageColorHex}>{playerName}</color></b> {message}";

        GameObject obj = Instantiate(chatMessagePrefab, scrollContent.transform);

        scrollBar.value = -1;

    }

    public void ClearChat()
    {
        foreach(Transform child in scrollContent.transform)
        {
            Destroy(child.gameObject);
        }

    }

}
