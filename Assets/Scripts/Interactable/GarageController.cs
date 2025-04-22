using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarageController : LightDependentElement
{
    [SerializeField] Garage garage;
    public bool isUnlocked;
    public bool canOpen;

    [SerializeField] AudioClip unlockingClip;

    public void OpenGate()
    {
        if (canOpen)
        {
            canOpen = false;
            garage.OpenGate();
        }
    }

    public void UnlockGate()
    {
        isUnlocked = true;
        AudioSource.PlayClipAtPoint(unlockingClip, transform.position);
        Debug.Log("unlocked");
    }

    public void LockGate()
    {
        isUnlocked = false;
        AudioSource.PlayClipAtPoint(unlockingClip, transform.position);
        Debug.Log("locked");
    }

    public override void OnLightTurnOff()
    {
        isPowered = false;
    }

    public override void OnLightTurnOn()
    {
        isPowered = true;
    }
}
