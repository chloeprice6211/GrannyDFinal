using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VoidObject : MonoBehaviour, IReceivePassword
{

    [SerializeField] TextMeshProUGUI textField;

    public void DisplayPassword(string code)
    {
        textField.text = code;
    }
}
