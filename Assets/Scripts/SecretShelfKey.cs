using Mirror;
using UnityEngine;
using UnityEngine.Events;

public class SecretShelfKey : Key
{   public UnityEvent takeEvent;
    public override void TakeItem(NetworkPlayerController owner)
    {
        base.TakeItem(owner);
        CmdInvokeTake();
    }

    [Command(requiresAuthority = false)]
    void CmdInvokeTake(){
        RcpInvokeTake();
    }

    [ClientRpc]
    void  RcpInvokeTake(){
        takeEvent.Invoke();
    }

}
