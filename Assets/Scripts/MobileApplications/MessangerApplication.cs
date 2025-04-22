using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MessangerApplication : MobileApplication
{
    [SerializeField] GameObject codeMessage;
    [SerializeField] TextMeshProUGUI textField;
    [SerializeField] GameObject noMessage;
 

    public void SetMessage(string message)
    {
        textField.text = message;  
    }

    public void ShowMessage()
    {
        codeMessage.SetActive(true);
        noMessage.SetActive(false);
    }

}
