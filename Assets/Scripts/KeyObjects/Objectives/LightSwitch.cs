using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSwitch : LightDependentElement, IInteractableRpc, IGhostInteractable
{
    [SerializeField] Animation _animation;
    [SerializeField] AudioClip switchSound;

    private int _interactions = 0;
    private bool _isOn;


    public void Interact(NetworkPlayerController owner)
    {
        if (_interactions == 0)
        {
            Invoke("ShowMessage", 1f);
        }

        _interactions++;

        SwitchLightCommand();

    }


    [Command (requiresAuthority = false)]
    public void SwitchLightCommand()
    {
        SwitchLightRpc();
    }
    [ClientRpc]
    private void SwitchLightRpc()
    {
        _isOn = !_isOn;
        AudioSource.PlayClipAtPoint(switchSound, transform.position, .2f);

        if (_isOn)
        {
            Debug.Log("OFF");
            _animation.Play("SwitchLightOn");
            tag = lightOnTag;
        }
        else
        {
            _animation.Play("SwitchLightOff");
            tag = lightOffTag;
        }
    }


    public void PerformGhostInteraction()
    {
        SwitchLightCommand();
    }


    private void ShowMessage()
    {
        UIManager.Instance.Message("notWorking", "powerlessv1_A");
    }

    public override void OnLightTurnOff()
    {
        
    }

    public override void OnLightTurnOn()
    {
        
    }
}
