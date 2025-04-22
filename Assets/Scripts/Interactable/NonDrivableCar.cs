using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonDrivableCar : MonoBehaviour, IInteractableRpc
{
    public string tip;

    public void Interact(NetworkPlayerController owner)
    {
        if (!owner.hasAuthority) return;

        //UIManager.Instance.Message(tip);
    }

}
