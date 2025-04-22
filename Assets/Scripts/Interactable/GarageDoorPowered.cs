using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarageDoorPowered : LightDependentElement, IUnlockable
{
    [SerializeField] Animation openAnimation;
    [SerializeField] AudioSource aSource;

    public bool Check()
    {
        if(isPowered) return true;
        return false;
    }

    public override void OnLightTurnOff()
    {
        isPowered = false;
    }

    public override void OnLightTurnOn()
    {
        isPowered = true;
    }

    public void OpenGate()
    {
        openAnimation.Play();
        aSource.Play();
    }

    public void Unseal()
    {   if(!isPowered) return;
        OpenGate();
    }
}
