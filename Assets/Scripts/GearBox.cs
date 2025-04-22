using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GearBox : NetworkBehaviour, IInteractableRpc, IUnlockable, IHighlight
{
    [SerializeField] Outline outline;
    [SerializeField] Animation gearAnim;
    [SerializeField] Transform gearPosition;
    [SerializeField] Garage garage;
    [SerializeField] AudioClip operateGearClip;
    [SerializeField] AudioClip gearInstallClip;

    bool _isGearOperating;
    bool _hasGear;
    bool _hasBeenOpened;

    public void ChangeHighlightThickness(float value)
    {
        
    }

    public bool Check()
    {
        throw new System.NotImplementedException();
    }

    public void DisableHighlight()
    {
        if (outline != null)
            outline.enabled = false;
    }

    public void Highlight()
    {
        if (outline != null)
            outline.enabled = true;
    }

    public void Interact(NetworkPlayerController owner)
    {
        if (!_hasGear && owner.hasAuthority)
        {
            if (Inventory.Instance.HasAnyItem(owner))
            {
                Item gear = Inventory.Instance.GetMainItem(owner);

                if (gear is GearPart)
                {
                    owner.InventoryScript.ClearItem(owner.InventoryScript.items.IndexOf(gear));
                    //(gear as GearPart).OnDropItem();
                    UIManager.Instance.Message("workProperly", "shouldWorkV2_A");
                    InsertGearCmd(gear as GearPart);

                    return;
                }
            }
        }
    }

    public void OperateGear()
    {
        if (_hasBeenOpened) return;
        if (gearAnim.isPlaying) return;
        AudioSource.PlayClipAtPoint(operateGearClip, transform.position);

        gearAnim.Play();
        if(_hasGear) garage.Unseal();

        if (_hasGear)
        {
            _hasBeenOpened = true;
        } 
    }

    public void Unseal()
    {
        OperateGear();
    }

    [Command (requiresAuthority = false)]
    void InsertGearCmd(GearPart gear)
    {
        InsertGearRpc(gear);
    }

    [ClientRpc]
    void InsertGearRpc(GearPart gear)
    {
        Transform parentItem = gear.transform.parent;

        parentItem.SetParent(gearPosition);
        parentItem.localPosition = Vector3.zero;
        parentItem.localRotation = Quaternion.Euler(new Vector3(90,0,0));

        gear.gameObject.layer = 0;

        AudioSource.PlayClipAtPoint(gearInstallClip, transform.position);

        _hasGear = true;
    }
}
