using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableLamp : LVL2Lamp, IInteractableRpc
{
    [SerializeField] AudioClip switchSound;

    public void Interact(NetworkPlayerController owner)
    {
        AudioSource.PlayClipAtPoint(switchSound, transform.position);

        if (!isPowered)
            UIManager.Instance.Message("powerless", "powerless_A");

        Switch();
    }
}
