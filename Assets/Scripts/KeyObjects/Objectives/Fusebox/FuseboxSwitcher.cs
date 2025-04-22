using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FuseboxSwitcher : Mirror.NetworkBehaviour, IInteractableRpc
{
    [SerializeField] Renderer glowingMaterial;
    [SerializeField] Light lightSource;
    [SerializeField] Animation switcherAnimation;
    [SerializeField] AudioClip switchSound;
    [SerializeField] Fusebox fusebox;

    [SerializeField] Material switchenOnMaterial;
    [SerializeField] Material switchenOffMaterial;

    public UnityEvent OnLightOn;
    public UnityEvent OnLightOff;

    public bool isOn;


    private void Start()
    {
        if (isOn) Invoke("TurnOn",0.1f);
        else Invoke("TurnOff",0.1f);
    }


    public void OnUISwitch()
    {
        if (fusebox.isLocked)
        {
            UIManager.Instance.Message("blockedSwitchers", "blockedSwitchers_A");
            return;
        }

        SwitchCommand();
    }

    public void Interact(NetworkPlayerController owner)
    {
        if (fusebox.isLocked)
        {
            //UIManager.Instance.Message("Switchers are blocked. The panel says I need a phone..");
            return;
        }

        SwitchCommand();
    }

    #region Switch client/server

    [Command (requiresAuthority = false)]
    public void SwitchCommand()
    {
        SwitchRpc();
    }

    [ClientRpc]
    private void SwitchRpc()
    {
        Switch();
    }

    public void Switch()
    {
        AudioSource.PlayClipAtPoint(switchSound, transform.position, .2f);

        if (isOn)
        {
            TurnOff();
        }
        else
        {
            TurnOn();
        }
    }

    #endregion

    public virtual void TurnOn()
    {
        fusebox.hasRecentlyBeenOff = false;

        tag = "LightOff";
        glowingMaterial.material = switchenOnMaterial;
        switcherAnimation.Play("FuseboxSwitch");
        lightSource.color = Color.green;

        isOn = true;
        OnLightOn?.Invoke();
        
    }

    public void TurnOff()
    {
        tag = "LightOn";
        glowingMaterial.material = switchenOffMaterial;
        switcherAnimation.Play("FuseboxSwitchOff");
        lightSource.color = Color.red;

        isOn = false;
        OnLightOff?.Invoke();
        
    }
}
