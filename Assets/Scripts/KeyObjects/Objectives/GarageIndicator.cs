using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarageIndicator : LightDependentElement
{
    [SerializeField] GarageDoor garageDoor;
    [SerializeField] GarageFence fence;
    [SerializeField] Light lightSource;

    [SerializeField] Material inactiveMaterial;
    [SerializeField] Material unlockedMaterial;
    [SerializeField] Material lockedMaterial;

    [SerializeField] Renderer _renderer;

    [SerializeField] AudioClip unlockClip;

    public bool hasGate;


    public void TurnOff()
    {
        _renderer.material = inactiveMaterial;
        lightSource.enabled = false;
    }

    public void TurnOn()
    {
        if (garageDoor.isUnlocked)
        {
            _renderer.material = unlockedMaterial;
            lightSource.color = Color.green;
        }
        else
        {
            _renderer.material = lockedMaterial;
            lightSource.color = Color.red;
        }

        lightSource.enabled = true;
    }

    public bool Lock()
    {
        if (!isPowered)
        {
            UIManager.Instance.Message("garaageDoorPowerless", "garagePowerlesslvl1_A");
            return false;
        }
        if (fence.IsFenceOperating)
        {
            return false;
        }
        if (!garageDoor.isUnlocked)
        {
            return false;
        }

        if (hasGate)
        {
            fence.Lock();
        }
        

        garageDoor.isUnlocked = false;
        TurnOn();

        return true;
    }

    public bool Unlock()
    {
        if (!isPowered)
        {
            UIManager.Instance.Message("garaageDoorPowerless", "garagePowerlesslvl1_A");
            return false;
        }
        if (garageDoor.isUnlocked)
        {
            return false;
        }

        if (fence.IsFenceOperating)
        {
            return false;
        }

        AudioSource.PlayClipAtPoint(unlockClip, transform.position, .7f);

        if (hasGate)
        {
            fence.Unlock();
        }
       
        garageDoor.isUnlocked = true;
 
        TurnOn();

        return true;
    }

    public void OpenGate()
    {
        garageDoor.Open();
    }

    public override void OnLightTurnOff()
    {
        isPowered = false;
        TurnOff();
    }

    public override void OnLightTurnOn()
    {
        isPowered = true;
        TurnOn();
    }
}
