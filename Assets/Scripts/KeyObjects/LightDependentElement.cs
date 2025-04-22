using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public abstract class LightDependentElement : NetworkBehaviour
{
    public bool isPowered;
    public const string lightOnTag = "LightOff";
    public const string lightOffTag = "LightOn";
        
    public abstract void OnLightTurnOff();
    public abstract void OnLightTurnOn();
}
