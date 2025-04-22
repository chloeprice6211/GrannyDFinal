using System.Collections.Generic;
using System.Threading;
using Mirror;
using UnityEngine;

public class ScreamerManager : NetworkBehaviour
{
    //this is a test
    bool isactive = true;
    //bool isdooractive = true;
    bool isgarageactive = true;
    public List<DoorScreamer> screamerDoors;
    public List<DoorScreamer> closedScreamerDoors = null;

    public Collider doorCOllider;
    public Collider garageCollider; 

    float _timer = 0f;

    void Start()
    {
        
    }

  [ServerCallback]
    void Update()
    {
        _timer+=Time.deltaTime;

        if(_timer >=400 && isactive){
            DetermineScreamerdoor();
            ActiveDoorScreamer();
            isactive = false;
        }

        if(_timer >= 540 && isgarageactive){
            isgarageactive = false;
            Debug.Log("garageActive");
            ActiveGarageScreamer();
        }
    }
    
    [Command (requiresAuthority =false)]
    void DetermineScreamerdoor(){
        foreach(DoorScreamer screamerdoor in screamerDoors){
            if(screamerdoor.door.isSealed){
                Debug.Log("added");
                closedScreamerDoors.Add(screamerdoor);
            }
        }

        if(closedScreamerDoors.Count == 0){
            Debug.Log("returned");
            return;
        }
        DoorScreamer door = closedScreamerDoors[Random.Range(0, closedScreamerDoors.Count-1)];

        Debug.Log("closed door is " + door.gameObject.name);
        SetScreamerDoor(door);
    }

    [ClientRpc]
    void SetScreamerDoor(DoorScreamer door1){
        Debug.Log(door1.gameObject.name + " is active");
        door1.isActive = true;
    }

    [Command (requiresAuthority = false)]
    void ActiveDoorScreamer(){
        ActivateDoorRpc();
    }
    [ClientRpc]
    void ActivateDoorRpc(){
        doorCOllider.enabled = true;
    }

      [Command (requiresAuthority = false)]
    void ActiveGarageScreamer(){
        GaragescreamerRpc();
    }
    [ClientRpc]
    void GaragescreamerRpc(){
        garageCollider.enabled = true;
    }
}
