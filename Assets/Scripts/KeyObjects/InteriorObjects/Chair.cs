using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Chair : Mirror.NetworkBehaviour, IAlternativeInteractable, IInteractableRpc
{
    [SerializeField] GameObject player;

    private Rigidbody _rb;

    public PlayerControls chairControls;
    public static GameObject ownerObj;

    public Vector3 testVector;

    private void Awake()
    {
        chairControls = new PlayerControls();

        chairControls.Chair.Release.performed += ReleasePerformed; 

    }

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public void Interact(NetworkPlayerController owner)
    {
        ownerObj = owner.gameObject;
        chairControls.Chair.Enable();

        TakeChairCommand(owner);
        
        UIManager.Instance.ShowNoRayInteraction("DROP", Keys.Space);

    }

    public void AlternativeInteract(NetworkPlayerController owner)
    {
        owner.controller.enabled = false;
        owner.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + 2f, this.transform.position.z);
        owner.controller.enabled = true;
    }

    #region Take/release client/server

    [Command (requiresAuthority = false)]
    private void TakeChairCommand(NetworkPlayerController owner)
    {
        TakeChairRpc(owner);
    }

    [ClientRpc]
    private void TakeChairRpc(NetworkPlayerController owner)
    {
        transform.SetParent(owner.GetComponent<NetworkPlayerController>().chairPosition);
        //_rb.isKinematic = true;
        transform.localPosition = new Vector3(0, 0, 0);
        transform.localRotation = Quaternion.Euler(0, 0, 0);

    }

    [Command (requiresAuthority = false)]
    private void ReleaseClientCommand()
    {
        ReleaseClientRpc();
    }

    [ClientRpc]
    private void ReleaseClientRpc()
    {
        transform.parent = null;
       // _rb.isKinematic = false;
    }

    #endregion

    private void ReleasePerformed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        UIManager.Instance.HideNoRayInteraction();

        ReleaseClientCommand();
    }
}
