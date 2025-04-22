using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractableRpc
{
    public void Interact(NetworkPlayerController owner);
}
