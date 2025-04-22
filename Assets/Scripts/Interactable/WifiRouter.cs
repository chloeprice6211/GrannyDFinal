using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WifiRouter : Router
{
    IReceivePassword _iObjectToReceive;
    [SerializeField] GameObject objectToReceive;

    public override void ShowGeneratedCode()
    {
        if(objectToReceive.TryGetComponent(out _iObjectToReceive))
            _iObjectToReceive.DisplayPassword("wifi: " + passcode);
    }
}
