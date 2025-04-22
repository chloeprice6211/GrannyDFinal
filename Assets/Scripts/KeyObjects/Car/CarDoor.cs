using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CarDoor : Door
{
    [SerializeField] Light lightSource;
    [SerializeField] Renderer _renderer;

    [SerializeField] Material glowingMaterial;
    [SerializeField] Material inactiveMaterial;

    [SerializeField] Car car;


    public override void OpenDoor()
    {
        base.OpenDoor();

        Debug.Log("happened");
        _renderer.material = glowingMaterial;
        lightSource.enabled = true;
    }

    public override void CloseDoor()
    {
        base.CloseDoor();
        Invoke("TurnOffDoorLight", .5f);
    }

    public override void UnlockDoor(Item key)
    {
        base.UnlockDoor(key);
        car.UnlockCarDoors();
    }

    private void TurnOffDoorLight()
    {
        _renderer.material = inactiveMaterial;
        lightSource.enabled = false;
    }
}
