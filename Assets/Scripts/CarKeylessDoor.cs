using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class CarKeylessDoor : NetworkBehaviour, IInteractableRpc
{
    [SerializeField] Animation carLockAnimation;
    [SerializeField] AudioClip unlockAC;

    public List<Door> carDoors;
    Collider _collider;

    Item _key;
    bool _hasBeenOpened;

    private void Start()
    {
        _collider = GetComponent<Collider>();
    }

    public void Interact(NetworkPlayerController owner)
    {
        if (_hasBeenOpened) return;

        if(Inventory.Instance.GetMainItem(owner) != null)
        {
            _key = Inventory.Instance.GetMainItem(owner);

            if(_key is Key && (_key as Key).objectiveType == ObjectiveType.Car)
            {
                UnlockCmd();
            }
            else
            {
                UIManager.Instance.Message("useAKey", "useKey_A");
            }
        }
        else
        {
            UIManager.Instance.Message("useAKey", "useKey_A");
        }
    }

    [Command  (requiresAuthority =  false)]
    void UnlockCmd()
    {
        UnlockRpc();
    }
    [ClientRpc]
    void UnlockRpc()
    {
        _hasBeenOpened = true;

        foreach (Door door in carDoors)
            door.isSealed = false;

        carLockAnimation.Play();
        AudioSource.PlayClipAtPoint(unlockAC, transform.position, .6f);

        //_collider.enabled = false;
        gameObject.layer = 0;
    }
}
