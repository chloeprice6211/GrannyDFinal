using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReviveHitbox : MonoBehaviour, IAlternativeInteractable
{
    Syringe _syringe;
    public Collider _collider;
    public NetworkPlayerController player;

    public void AlternativeInteract(NetworkPlayerController owner)
    {
        if(Inventory.Instance.GetMainItem(owner) is Syringe)
        {
            _syringe = Inventory.Instance.GetMainItem(owner) as Syringe;

            if(_syringe.uses == 0)
            {
                UIManager.Instance.Message("noInjector", "noInjector_A");
                Debug.Log("no uses");
                return;
            }

            _syringe.Use(player);
            Debug.Log("syringe!");
        }
    }

    
}
