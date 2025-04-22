using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntraceDoor : Door
{
    bool _hasBeenSealedByGhost;

    public override void Start()
    {
        GhostEvent.Instance.OnHuntStart.AddListener(OnHuntServerStart);
        GhostEvent.Instance.OnHuntEnd.AddListener(HandleGhostHuntEnd);
        base.Start();
    }

    [ServerCallback]
    void OnHuntServerStart()
    {
        if (Random.Range(0, 100) > 100)
            HandleGhostHuntStart();
    }

    [ClientRpc]
    void HandleGhostHuntStart()
    {
        if (isSealed) return;

        if (isClosed) CloseDoor();
        isSealed = true;
        _hasBeenSealedByGhost = true;


    }

    void HandleGhostHuntEnd()
    {
        if (_hasBeenSealedByGhost)
        {
            isSealed = false;
            _hasBeenSealedByGhost = false;
        }
    }
}
