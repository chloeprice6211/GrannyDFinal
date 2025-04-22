using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LVL2Switcher : NetworkBehaviour, IInteractableRpc, IGhostInteractable, IHighlight
{
    [SerializeField] Outline outline;

    [SerializeField] List<LVL2Lamp> lamps;
    [SerializeField] Animation switchAnimation;
    [SerializeField] AudioClip switchAudio;

    [SerializeField] AnimationClip switchOnAnimClip;
    [SerializeField] AnimationClip switchOffAnimClip;

    [SerializeField] Transform switchSoundPosition;

    public bool isOn;

    public const string lightOnTag = "LightOff";
    public const string lightOffTag = "LightOn";

    public void Switch()
    {
        CmdSwitch();
    }

    public void Interact(NetworkPlayerController owner)
    {
        Switch();
    }

    [Command (requiresAuthority = false)]
    void CmdSwitch()
    {
        RpcSwitch();

    }
    [ClientRpc]
    void RpcSwitch()
    {
        AudioSource.PlayClipAtPoint(switchAudio, switchSoundPosition.position, .35f);
        isOn = !isOn;

        if (isOn)
        {
            switchAnimation.Play(switchOnAnimClip.name);
            tag = lightOnTag;
        }
        else
        {
            switchAnimation.Play(switchOffAnimClip.name);
            tag = lightOffTag;
        }

        foreach (LVL2Lamp lamp in lamps)
        {
            lamp.Switch();
        }
    }

    public void PerformGhostInteraction()
    {
        CmdSwitch();
    }

    public void Highlight()
    {
        if (outline != null)
            outline.enabled = true;
    }

    public void DisableHighlight()
    {
        if (outline != null)
            outline.enabled = false;
    }

    public void ChangeHighlightThickness(float value)
    {
       
    }
}
