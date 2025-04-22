using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using UnityEngine.UIElements;

public class CarFixArea : Mirror.NetworkBehaviour, IInteractableRpc
{
    [Header("positions")]
    public Transform carJackPos;
    public Transform toolboxPos;
    public Transform tirePos;

    [Header("audio clips")]
    [SerializeField] AudioClip carJackInstallSound;
    [SerializeField] AudioClip toolboxInstallSound;
    [SerializeField] AudioClip tireInstallSound;
    [SerializeField] Wrench wrench;

    public Wheel wheel;

    private Item _item;
    private Tool _toolItem;
    private Collider _collider;

    [SyncVar] public bool isWheelInstalled;
    [SyncVar] public bool isCarJackInstalled;

    public float wheelInstalationTime = 5f;


    private void Awake()
    {
        _collider = GetComponent<Collider>();
    }

    public void Interact(NetworkPlayerController owner)
    {
        _item = Inventory.Instance.GetMainItem(owner);

        if (_item == null || _item.GetComponent<Item>() is not Tool) return;

        _toolItem = _item as Tool;

        if (_toolItem is Wheel)
        {
            if (isCarJackInstalled)
            {
                owner.InventoryScript.ClearItem(owner.InventoryScript.items.IndexOf(_toolItem), false);
                CmdSetUpTool(_toolItem, tirePos);
            }
            else
            {
                UIManager.Instance.Message("useCarJack", "needCarJack_A");
            }
        }
    }

    [Command (requiresAuthority = false)] 
    void CmdSetUpTool(Item tool, Transform position)
    {
        RpcSetUpTool(tool, position);
    }

    [ClientRpc]
    void RpcSetUpTool(Item tool, Transform pos)
    {
        Destroy(_collider);

        isWheelInstalled = true;
        wrench.canCastCustomRay = true;
        wheel.isInstalled = true;

        tool.OnDropItem();
        tool.transform.parent.SetParent(pos);
        tool.transform.parent.localPosition = Vector3.zero;
        tool.transform.parent.localRotation = Quaternion.identity;

        if (tool is IInstallable)
        {
            tool.GetComponent<IInstallable>().OnInstall();
        
        
        }

        gameObject.tag = "Untagged";

        Invoke("ReturnTag", 1f);
    }

    void ReturnTag()
    {
        gameObject.tag = "CarFixArea";
    }
}