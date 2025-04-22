using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewKeypadDigit : HighlightElement
{
    [SerializeField] NewKeypad keypad;

    public char digit;

    public void AddDigitToKeypad()
    {
        keypad.AddDigit(digit);
    }
}
