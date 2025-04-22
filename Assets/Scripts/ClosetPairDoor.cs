using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClosetPairDoor : Door
{
    public ClosetPairDoor pairedDoor;

    public override void Start()
    {
        base.Start();

        GhostEvent.Instance.OnHuntStart.AddListener(OnGhostEventStart);
        GhostEvent.Instance.OnHuntEnd.AddListener(OnGhostEventEnd);
    }

    public override void OpenDoor()
    {
        base.OpenDoor();
        //pairedDoor.OpenDoor();
    }

    [ServerCallback]
    private void OnGhostEventStart()
    {
        if (Random.Range(0, 100) < SetupPanel.LevelSettings.SealLockerDuringHuntChance)
        {
            isSealed = true;

            LockDuringHunt();
        }
    }

    [ClientRpc]
    void LockDuringHunt()
    {
        if (!isClosed)
        {
            CloseDoor();
        }
    }

    private void OnGhostEventEnd()
    {
        if (isSealed)
            isSealed = false;
    }
}
