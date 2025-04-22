using Mirror;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SwitcherElement : NetworkBehaviour ,IInteractableRpc
{
    [SerializeField] Animation switcherAnimation;

    [SerializeField] AnimationClip activateClip;
    [SerializeField] AnimationClip deactivateClip;

    [SerializeField] SwitchPanel panel;

    [Header("renderer")]
    [SerializeField] Material activeMaterial;
    [SerializeField] Material inactivaMaterial;
    [SerializeField] Renderer _renderer;
    [SerializeField] Light lightSource;
    [SerializeField] AudioClip activateSound;

    public bool isOn;
    public int index;


    public void Interact(NetworkPlayerController owner)
    {
        Activate();
    }

    public void Activate()
    {
        ActivateCmd();

    }

    [Command (requiresAuthority = false)]
    void ActivateCmd()
    {
        ActivateRpc();
    }
    [ClientRpc]
    void ActivateRpc()
    {
        AudioSource.PlayClipAtPoint(activateSound, transform.position);
        isOn = !isOn;

        if (isOn)
        {
            switcherAnimation.Play(activateClip.name);
            _renderer.material = activeMaterial;
            lightSource.color = Color.green;
        }
        else
        {
            switcherAnimation.Play(deactivateClip.name);
            _renderer.material = inactivaMaterial;
            lightSource.color = Color.red;
        }

        panel.ActivateSwitcher(index);
    }
}
