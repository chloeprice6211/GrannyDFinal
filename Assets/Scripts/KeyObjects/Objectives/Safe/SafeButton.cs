using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeButton : NetworkBehaviour, IInteractableRpc
{
    public Safe safe;

    [SerializeField] char combinationDigit;
    [SerializeField] AudioClip pressButton;


    public void Interact(NetworkPlayerController owner)
    {
        CmdAddDigit();
    }

    [Command (requiresAuthority = false)]
    void CmdAddDigit()
    {
        RpcAddDigit();
    }
    [ClientRpc]
    void RpcAddDigit()
    {
        AudioSource.PlayClipAtPoint(pressButton, transform.position, .5f);
        safe.AddCombinationDigit(combinationDigit);
    }
}
