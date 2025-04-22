using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using static UnityEngine.UI.GridLayoutGroup;
using Unity.VisualScripting.Antlr3.Runtime.Misc;

public class Backpack : NetworkBehaviour, IInteractableRpc
{
    [SerializeField] AudioClip equipSound;

    public int Slots;

    private void Awake()
    {
        Slots = CustomNetworkManager.Instance.LobbyPlayers.Count > 1 ? 1 : 2;
    }

    public void Interact(NetworkPlayerController owner)
    {
        Inventory inv = owner.InventoryScript;

        if (inv.HasBackpack) return;
        EquipBackpack(owner);

        UIManager.Instance.uiInventory.OnBackpackPickup(Slots);

        for(int a = 0; a < Slots; a++)
        {
            owner.InventoryScript.items.Add(null);
        }
    }
    [Command (requiresAuthority = false)]
    void EquipCmd(NetworkPlayerController owner, int currSlots, int maxslots)
    {
        EquipRpc(owner, currSlots, maxslots);
    }

    [ClientRpc]
    void EquipRpc(NetworkPlayerController owner, int currSlots, int maxslots)
    {
        AudioSource.PlayClipAtPoint(equipSound, transform.position, .7f);
        GetComponent<BoxCollider>().enabled = false;

        if (owner.hasAuthority)
        {
            //UIManager.Instance.DisplayFadeTip("backpackEquip");
        }
        Transform _parent = transform.parent;

        _parent.SetParent(owner.pelvis, true);
        _parent.localPosition = Vector3.zero;
        _parent.localRotation = Quaternion.identity;

        Inventory inv = owner.InventoryScript;

        inv.HasBackpack = true;
        inv.MaxSlots = maxslots;
        inv.CurrentSlots = currSlots;

    }

    void EquipBackpack(NetworkPlayerController owner)
    {
        Inventory inv = owner.InventoryScript;

        inv.HasBackpack = true;
        inv.MaxSlots += Slots;
        inv.CurrentSlots += Slots;
        EquipCmd(owner, inv.CurrentSlots, inv.MaxSlots);
    }
}
