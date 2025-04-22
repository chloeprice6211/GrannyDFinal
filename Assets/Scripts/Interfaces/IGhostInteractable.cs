using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGhostInteractable
{
    /// <summary>
    /// returns 1 if unsuccessfull, otherwise 0
    /// </summary>
    /// <returns></returns>
    public void PerformGhostInteraction();
}
