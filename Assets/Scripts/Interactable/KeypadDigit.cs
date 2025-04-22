using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeypadDigit : NetworkBehaviour, IInteractableRpc
{
    [SerializeField] Keypad keypad;
    [SerializeField] char digit;
    [SerializeField] AudioClip interactSound;

    public void Interact(NetworkPlayerController owner)
    {
        CmdPress();
    }

    [Command (requiresAuthority = false)]
    void CmdPress()
    {
        RpcPress();
    }
    [ClientRpc]
    void RpcPress()
    {
        keypad.AddDigit(digit);
        AudioSource.PlayClipAtPoint(interactSound, transform.position);
    }
}
