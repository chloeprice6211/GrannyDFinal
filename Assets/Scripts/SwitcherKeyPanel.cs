using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitcherKeyPanel : NetworkBehaviour, IInteractableRpc
{
    [SerializeField] Transform keyInstallPosition;
    [SerializeField] Animation keyInstallAnimation;
    [SerializeField] SwitchPanel panel;
    [SerializeField] AudioClip insert;
    [SerializeField] ItemList receivedItem;

    public void Interact(NetworkPlayerController owner)
    {
        if (Inventory.Instance.HasAnyItem(owner))
        {
            Item _item = Inventory.Instance.GetMainItem(owner);

            if(_item is SwitcherKey)
            {
                UIManager.Instance.HideInteractOption();
                owner.InventoryScript.ClearItem(owner.InventoryScript.items.IndexOf(_item), false);
                CmdInstall(_item);
                _item.OnDropItem();
            }
        }
    }

    [Command (requiresAuthority = false)]
    void CmdInstall(Item item)
    {
        RpcInstall(item);
    }
    [ClientRpc]
    void RpcInstall(Item _item)
    {
        Transform parent = _item.transform.parent;

        parent.parent = keyInstallPosition;
        parent.localPosition = Vector3.zero;
        parent.localRotation = Quaternion.identity;

        keyInstallAnimation.Play();
        panel.isKeyInstalled = true;
        _item.GetComponent<Collider>().enabled = false;

        AudioSource.PlayClipAtPoint(insert, transform.position);
    }
}
