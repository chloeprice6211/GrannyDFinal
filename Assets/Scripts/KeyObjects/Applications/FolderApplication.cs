using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FolderApplication : DesktopApplication, IReceivePassword
{
    [SerializeField] TextMeshProUGUI passcodeTF;

    public void DisplayPassword(string code)
    {
        Debug.Log("happened");

        for(int a = 0; a < code.Length; a++)
        {
            passcodeTF.text += code[a];
            if(a%2 != 0 && a != 0 && a!= code.Length-1)
            {
                passcodeTF.text += "-";
            }
        }

      //  passcodeTF.text = code;
    }
}
