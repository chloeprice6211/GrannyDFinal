using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NewPC : LightDependentElement, IInteractableRpc
{
    public bool isOn;

    public NewMonitor monitor;

    [SerializeField] AudioClip switchSound;
    [SerializeField] AudioClip pcStartupSound;

    [SerializeField] AudioSource audioSource;

    [SerializeField] Light lightSource;

    public void Interact(NetworkPlayerController owner)
    {
        AudioSource.PlayClipAtPoint(switchSound, transform.position, .5f);

        if (!isPowered) return;

        if (!isOn)
            TurnOnCommand();
        else
            TurnOffCommand();
    }

    #region turn c/s

    [Command(requiresAuthority = false)]
    public void TurnOnCommand()
    {
        TurnOnRpc();
    }
    [ClientRpc]
    private void TurnOnRpc()
    {
        TurnOnPC();
    }

    private void TurnOnPC()
    {
        isOn = true;

        audioSource.Play();
        AudioSource.PlayClipAtPoint(pcStartupSound, transform.position);

        tag = lightOnTag;
        lightSource.enabled = true;

        monitor.TurnMonitorOn();

    }

    [Command(requiresAuthority = false)]
    public void TurnOffCommand()
    {
        TurnOffRpc();
    }
    [ClientRpc]
    private void TurnOffRpc()
    {
        TurnOffPc();
    }

    private void TurnOffPc()
    {
        isOn = false;
        tag = lightOffTag;

        audioSource.Stop();

        lightSource.enabled = false;

        monitor.TurnMonitorOff();
    }

    #endregion

    #region events

    public override void OnLightTurnOn()
    {
        isPowered = true;

        if (isOn)
        {
            TurnOnPC();
        }
    }

    public override void OnLightTurnOff()
    {
        isPowered = false;

        if (isOn)
        {
            TurnOffPc();
        }
    }

    #endregion

}
