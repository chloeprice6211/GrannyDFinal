using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExceptionManager : MonoBehaviour
{
    [SerializeField] UserDataCollector dataCollector;

    void Awake()
    {
        Application.logMessageReceived += LogCaughtException;
        DontDestroyOnLoad(gameObject);
    }

    void LogCaughtException(string logText, string stackTrace, LogType logType)
    {
        if (Application.isEditor) return;

        if (logType == LogType.Exception || logType == LogType.Error)
        {   
            dataCollector.SendFeedback($"{logText} \n\n {stackTrace}",
                UserDataCollector.FeedbackType.Error);
        }
    }


}
