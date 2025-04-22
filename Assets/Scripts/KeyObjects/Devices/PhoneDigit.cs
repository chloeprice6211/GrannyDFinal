using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhoneDigit : MonoBehaviour
{
    [SerializeField] Smartphone phone;
    [SerializeField] Animation _digitAnimation;
    
    public int digit;

    public void OnButtonClick()
    {
        _digitAnimation.Play();
        phone.AddCombinationDigit(digit);

        Debug.Log(digit);
    }
}
