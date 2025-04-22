using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITakeable
{
    public void TakeItem(NetworkPlayerController owner);
    public void OnDropItem();
}
