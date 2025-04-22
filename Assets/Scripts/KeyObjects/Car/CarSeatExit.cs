using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSeatExit : Mirror.NetworkBehaviour, IInteractableRpc
{
    [SerializeField] CarSeat carSeat;
    private Car car;
    private Collider _collider;


    private void Awake()
    {
        car = carSeat.car;
        _collider = GetComponent<Collider>();
    }

    public void Interact(NetworkPlayerController owner)
    {
        car.ExitCar();
    }

    public void EnableOrDisableExitCollider()
    {
        _collider.enabled = !_collider.enabled;
    }
}
