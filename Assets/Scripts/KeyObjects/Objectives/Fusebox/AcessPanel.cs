using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcessPanel : Mirror.NetworkBehaviour, IInteractableRpc
{
    [SerializeField] Renderer _renderer;
    [SerializeField] Material unlockedMaterial;

    [SerializeField] AudioClip success;
    [SerializeField] AudioClip cardInsert;

    [SerializeField] Door door;
    [SerializeField] Transform keyInstallPosition;
    [SerializeField] Animation keyHoleAnimation;

    [SerializeField] LayerMask defaultLayer;

    public ObjectiveType objectiveType;
    public bool isSealed = true;

    private Item _key;

    public void Interact(NetworkPlayerController owner)
    {
        if (!owner.hasAuthority) return;

        if (isSealed)
        {
            Transform playerInventory = Inventory.Instance.GetPlayerInventory(owner);

            if (playerInventory.childCount > 0)
            {
                _key = Inventory.Instance.GetMainItem(owner);

                if (_key.GetComponent<ITakeable>() is KeyCard)
                {
                    if (_key.GetComponent<KeyCard>().objectiveType == objectiveType)
                    {
                        owner.InventoryScript.ClearItem(owner.InventoryScript.items.IndexOf(_key));
                        InsertCardCommand(_key);
                        GetComponent<Collider>().enabled = false;
                    }
                    else
                    {
                        UIManager.Instance.Message("useKeycard", "useKeycard_A");
                    }
                }
                else
                {
                    UIManager.Instance.Message("useKeycard", "useKeycard_A");
                }
            }
            else
            {
                UIManager.Instance.Message("useKeycard", "useKeycard_A");
            }
        }
    }

    #region functionality client/server

    [Command (requiresAuthority = false)]
    public void InsertCardCommand(Item key)
    {
        InsertCardRpc(key);
    }
    [ClientRpc]
    private void InsertCardRpc(Item key)
    {
        InsertCard(key);
    }

    private void InsertCard(Item _key)
    {
        AudioSource.PlayClipAtPoint(cardInsert, keyInstallPosition.position, .5f);
        Invoke("UnlockAccessPanel", 1f);

        if (_key.inspectAnimation.isPlaying)
        {
            _key.inspectAnimation.Stop();
        }

        _key.transform.parent.SetParent(keyInstallPosition);
        _key.transform.parent.localPosition = Vector3.zero;
        _key.transform.parent.localRotation = Quaternion.identity;
        _key.GetComponent<Collider>().enabled = false;
        keyHoleAnimation.Play();
    }


    [Command (requiresAuthority = false)]
    public void UnlockAccessPanelCommand()
    {
        UnlockAccessPanelRpc();
    }
    [ClientRpc]
    private void UnlockAccessPanelRpc()
    {
        UnlockAccessPanel();
    }

    public void UnlockAccessPanel()
    {
        gameObject.layer = defaultLayer;
        //UIManager.Instance.Message("Got it.");
        UnlockDoor();
        isSealed = false;
        _renderer.material = unlockedMaterial;

        AudioSource.PlayClipAtPoint(success, transform.position);

    }

    public void UnlockDoor()
    {
        door.isSealed = false;
    }

    #endregion
}
