using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockerPair : NetworkBehaviour
{
    public List<Locker> lockers;

    private void Start()
    {
        GhostEvent.Instance.OnHuntStart.AddListener(OnHunt);
        GhostEvent.Instance.OnHuntEnd.AddListener(OnHuntEnd);
    }

    [ServerCallback]
    void OnHunt()
    {
        LockLockers();
    }

    void OnHuntEnd()
    {
        foreach(Locker locker in lockers)
        {
            locker.isSealed = false;
        }
    }

    void LockLockers()
    {
        if(Random.Range(0,100) < SetupPanel.LevelSettings.SealLockerDuringHuntChance)
        {
            LockLockersRpc();
        }
    }

    [ClientRpc]
    void LockLockersRpc()
    {
        foreach (Locker locker in lockers)
        {
            locker.isSealed = true;

            if (!locker.isClosed)
            {
                locker.CloseDoor();
            }
        }
    }
}
