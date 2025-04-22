using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : NetworkBehaviour, IInteractableRpc
{
    [SerializeField] SwitchPanel panel;
    [SerializeField] Animation leverAnimation;
    [SerializeField] AudioClip audioClip;

    IUnlockable _iUnlockableElement;
    [SerializeField] GameObject unlockObject;

    private void Start()
    {
        unlockObject.TryGetComponent(out _iUnlockableElement);
    }

    public void Interact(NetworkPlayerController owner)
    {
        if (leverAnimation.isPlaying)
        {
            return;
        }

        if(_iUnlockableElement != null)
        {
            ActivateCmd();
        }
    }

    [Command (requiresAuthority = false)]
    void ActivateCmd()
    {
        ActivateRpc();
    }
    [ClientRpc]
    void ActivateRpc()
    {
        Activate();
    }

    public virtual void Activate()
    {
        leverAnimation.Play();
        AudioSource.PlayClipAtPoint(audioClip, transform.position, .5f);
        //panel.ActivateLever();

        _iUnlockableElement.Unseal();
    }
}
