using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trunk : Door
{
    [SerializeField] Light lightSource;

    public override void Interact(NetworkPlayerController owner)
    {
        if (isSealed)
        {
            UIManager.Instance.Message("trunkOpen", "lvlTrunk_A");
            AudioSource.PlayClipAtPoint(noKeyAttemp, transform.position, .5f);
        }
        else
        {
            OpenCloseDoorCommand();
        }

    }

    public override void OpenDoor()
    {
        base.OpenDoor();
        lightSource.enabled = true;

    }

    public override void CloseDoor()
    {
        base.CloseDoor();
        Invoke("TurnLightOff", .4f);
    }

    private void TurnLightOff()
    {
        lightSource.enabled = false;
    }
}
