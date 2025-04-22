using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainButtons : NetworkBehaviour, IInteractableRpc
{
    [SerializeField] Safe safe;

    public void Interact(NetworkPlayerController gameObject)
    {
        CmdApply();
    }

    [Command (requiresAuthority = false)]
    void CmdApply()
    {
        RpcApply();
    }
    [ClientRpc]
    void RpcApply()
    {
        safe.ApplyCombination();
    }
}
