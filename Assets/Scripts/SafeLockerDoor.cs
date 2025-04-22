using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeLockerDoor : Door
{
    [SerializeField] Door pairDoor;

    public override void UnlockDoor(Item key)
    {
        base.UnlockDoor(key);

        pairDoor.isSealed = false;
    }
}
