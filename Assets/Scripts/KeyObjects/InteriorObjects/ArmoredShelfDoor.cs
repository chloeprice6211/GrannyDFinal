using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmoredShelfDoor : NetworkBehaviour, IInteractableRpc
{
    [SerializeField] GameObject shelfObject;
    [SerializeField] Transform keyInstallPosition;
    [SerializeField] Animation keyHoleAnimation;
    [SerializeField] AudioClip shelfUnlock;

    [SerializeField] AudioClip attempSound;

    public ObjectiveType objectiveType;

    private ArmoredShelf _shelf;


    private void Start()
    {
        _shelf = shelfObject.GetComponent<ArmoredShelf>();
    }

    public void Interact(NetworkPlayerController owner)
    {
        Item item = Inventory.Instance.GetMainItem(owner);

        if (item is null)
        {
            UIManager.Instance.Message("useAKey", "useKey_A");
            return;
        }

        if(item.GetComponent<ITakeable>() is Key)
        {
            if(item.GetComponent<Key>().objectiveType == objectiveType)
            {
                owner.InventoryScript.ClearItem(owner.InventoryScript.items.IndexOf(item));
                CmdUnlock(item as Key);
            }
            else
            {
                AudioSource.PlayClipAtPoint(attempSound, transform.position, .5f);
                UIManager.Instance.Message("useAnotherKey", "useAnotherKey_A");
            }
        }
    }

    [Command (requiresAuthority = false)]
    void CmdUnlock(Key key)
    {
        RpcUnlock(key);
    }
    [ClientRpc]
    void RpcUnlock(Key key)
    {
        Transform parentItem = key.transform.parent;

        parentItem.SetParent(keyInstallPosition);
        parentItem.localPosition = new Vector3(0, 0, 0);
        parentItem.localRotation = Quaternion.identity;

        key.GetComponent<Collider>().enabled = false;
        keyHoleAnimation.Play();
        AudioSource.PlayClipAtPoint(shelfUnlock, transform.position);

        Invoke("DelayedUnlock", 2f);
    }

    void DelayedUnlock()
    {
        GetComponent<Collider>().enabled = false;
        _shelf.OpenShelf();
    }
}
