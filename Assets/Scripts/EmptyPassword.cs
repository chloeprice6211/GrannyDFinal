using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EmptyPassword : MonoBehaviour, IReceivePassword
{
    [SerializeField] TextMeshProUGUI textField;
    //comment

#region 

#endregion
    public void DisplayPassword(string code)
    {
        textField.text = code;

        
    }
    
}
