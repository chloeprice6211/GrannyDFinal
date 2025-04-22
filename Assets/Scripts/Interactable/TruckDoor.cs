using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TruckDoor : Door
{
    public List<Door> truckDoors;
    public List<LVL2Lamp> carLights;

    public override void UnlockDoor(Item key)
    {
        base.UnlockDoor(key);

        foreach(Door door in truckDoors)
        {
            door.isSealed = false;
        }
    }

    public override void OpenDoor()
    {
        base.OpenDoor();
        foreach (LVL2Lamp light in carLights)
            if (!light.isOn) light.TurnOn();      
    }

    public override void CloseDoor()
    {
        base.CloseDoor();

        if (truckDoors.All(c => c.isClosed == true))
        {
            Debug.Log("all closed");

            foreach (LVL2Lamp lamp in carLights)
                lamp.TurnOff();
        }

    }
}
