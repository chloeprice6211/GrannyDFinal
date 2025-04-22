using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Book : Device
{
    [SerializeField] Animation pageAnimation;
    [SerializeField] AudioClip bookAudio;
    [SerializeField] AudioClip bookExitAudio;

    public override void UseDevice(InputAction.CallbackContext context)
    {
        if (!_owner.hasAuthority) return;
        if (pageAnimation.isPlaying) return;
        if (isBeingInspected) return;
        if (inspectAnimation.isPlaying) return;

        base.UseDevice(context);
        
        UseCmd();
    }
    public override void ExitDevice(InputAction.CallbackContext context)
    {
        if (!_owner.hasAuthority) return;
        if (pageAnimation.isPlaying) return;

        base.ExitDevice(context);
        ExitCmd();
    }

    [Command (requiresAuthority = false)]
    void UseCmd()
    {
        UseRpc();
    }
    [ClientRpc]
    void UseRpc()
    {
        pageAnimation.Play("bookOpen");
        AudioSource.PlayClipAtPoint(bookAudio, transform.position, .7f);
    }

    [Command (requiresAuthority = false)]
    void ExitCmd()
    {
        ExitRpc();
    }
    [ClientRpc]
    void ExitRpc()
    {
        AudioSource.PlayClipAtPoint(bookExitAudio, transform.position, .7f);
        pageAnimation.Play("bookClose");
    }


}
