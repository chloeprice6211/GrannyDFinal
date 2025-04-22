using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewTV : MonoBehaviour, IGhostInteractable, ITriggered
{
    [SerializeField] GameObject tvObject;

    public void OnTrigger()
    {
        TurnOn();
    }

    public void PerformGhostInteraction()
    {
        TurnOn();
    }

    public void TurnOn()
    {
        if (tvObject.activeInHierarchy) return;

        tvObject.SetActive(true);
    }
}
