using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LVL1EmailController :  MonoBehaviour,IReceivePassword
{
    [SerializeField] EmailLetter letter;

    public void DisplayPassword(string code)
    {
        letter.combination += code;
    }

    
}
