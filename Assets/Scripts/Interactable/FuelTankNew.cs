using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuelTankNew : NetworkBehaviour
{
    [SerializeField] GameObject refuelGameObject;
    public IRefualable refualeble;
    private Collider _collider;
    public float maxCapacity;
    public float fuelLevel = 0;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
        refuelGameObject.TryGetComponent<IRefualable>(out refualeble);
    }

    public bool FuelCar()
    {
        fuelLevel += Time.deltaTime * 20;
        if (fuelLevel >=maxCapacity)
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
        refualeble.Refill();
    }
}
