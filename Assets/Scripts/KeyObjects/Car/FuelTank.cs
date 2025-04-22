using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuelTank : Mirror.NetworkBehaviour
{
    [SerializeField] Car car;
    private Collider _collider;
    public float maxCapacity;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
    }

    public bool FuelCar()
    {
        car.fuelLevel += Time.deltaTime;
        if (car.fuelLevel >= maxCapacity)
        {
            CmdCompelteFueling();
            return true;
        }

        return false;
    }

    [Command (requiresAuthority = false)]
    void CmdCompelteFueling()
    {
        RpcCompleteFueling();
    }

    [ClientRpc]
    void RpcCompleteFueling()
    {
        Destroy(_collider);
        car.fuelLevel = maxCapacity;
    }
}
