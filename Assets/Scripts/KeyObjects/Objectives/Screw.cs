using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Screw : Mirror.NetworkBehaviour
{
    public float timeToUnscrew = 5;
    public bool isActive = true;

    [SerializeField] LayerMask layerMask;
    [SerializeField] Vent vent;
    [SerializeField] GameObject unscrewObject;

    private Rigidbody _rb;

    IUnscrewable element;


    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public void Unscrew()
    {
        timeToUnscrew -= Time.deltaTime;
        transform.Rotate(1f, 0,0);
        transform.Translate(-.00001f,0, 0);

        if(timeToUnscrew <= 0 && isActive)
        {
            isActive = false;
            CmdUnscrew();
          
        }
    }

    [Command (requiresAuthority = false)]
    void CmdUnscrew()
    {
        RpcUnscrew();
    }
    [ClientRpc]
    void RpcUnscrew()
    {
        if(vent != null)
        {
            vent.RemoveScrew();
        }
        else
        {
            if(unscrewObject.TryGetComponent(out element))
            {
                element.Unscrew();
            }
        }
        Invoke("DestroyScrew", 3f);

        _rb.isKinematic = false;
        gameObject.layer = layerMask;
    }

    public void DestroyScrew()
    {
        Destroy(gameObject);
    }
}
