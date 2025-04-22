using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Painting : NetworkBehaviour, IUnscrewable
{
    [SerializeField] GameObject paintingObject;

    public int screwCount;

    #region client server

    [Command (requiresAuthority = false)]
    void CmdRelaese()
    {
        RpcRelease();
    }

    [ClientRpc]
    void RpcRelease()
    {
        Release();
    }

    #endregion

    public void Release()
    {
        paintingObject.GetComponent<Rigidbody>().isKinematic = false;
    }

    public void Unscrew()
    {
        screwCount--;

        if (screwCount == 0)
            Release();
    }
}
