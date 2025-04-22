using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSeat : Mirror.NetworkBehaviour, IInteractableRpc
{
    public enum SeatType
    {
        Passenger,
        Driver,
        None
    }

    public Car car;
    public CarSeatExit carSeatExit;
    private Collider _collider;
    private NetworkPlayerController _owner;

    [SyncVar] public bool isTaken;

    [SerializeField] SeatType _seatType;
    public SeatType Type
    {
        get
        {
            return _seatType;
        }
        set
        {
            _seatType = value;
        }
    }


    private void Start()
    {
        _collider = GetComponent<Collider>();
    }

    public void Interact(NetworkPlayerController owner)
    {
        if (!owner.hasAuthority) return;

        if (isTaken) return;

        CmdInteract(owner);
    }

    [Command (requiresAuthority = false)]
    void CmdInteract(NetworkPlayerController owner)
    {
        RpcInteract(owner);
    }
    [ClientRpc]
    void RpcInteract(NetworkPlayerController owner)
    {
        car.EnterCar(owner, this);
    }

    public void FreeOrTakeCarSeat()
    {
       
        CmdFreeOrTake();
        Debug.Log(!isTaken);
    }
    [Command (requiresAuthority =false)]
    void CmdFreeOrTake()
    {   
        isTaken = !isTaken;
        RpcFreeOrTake();
        
    }
    [ClientRpc]
    void RpcFreeOrTake()
    {
        _collider.enabled = !_collider.enabled;
    }
}
