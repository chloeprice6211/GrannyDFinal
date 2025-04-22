using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FridgeDoor : Door
{
    [SerializeField] Fridge fridge;


    public override void OpenDoor()
    {
        base.OpenDoor();
        Debug.Log("happened");
        fridge.EnableLight();
    }

    public override void CloseDoor()
    {
        base.CloseDoor();
        Invoke("InvokeDelayed",1f);
    }

    public void InvokeDelayed()
    {
        fridge.DisableLight();
    }

   
}
