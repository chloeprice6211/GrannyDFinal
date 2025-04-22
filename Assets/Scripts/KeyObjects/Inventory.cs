using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class Inventory : NetworkBehaviour
{
    private static Inventory _instance;
    public static Inventory Instance
    {
        get
        {
            return _instance;
        }
    }

    [SerializeField] Transform keyObjectHolder;

    public Animation trailerAnimation;

    public static AudioClip grab;

    public List<Item> items;
    public Item currentItem;
    public int currentSlot;
    public int Prev;

    private RaycastHit _raycastHit;

    public bool HasBackpack;

    public NetworkPlayerController networkPlayer;

    public int MaxSlots;
    [SyncVar] public int CurrentSlots;

    private void Start()
    {
        _instance = this;
        currentSlot = 0;

    }

    public bool TakeItemClient(Item newItem, NetworkPlayerController owner)
    {
        if (CheckForUtility(newItem, owner)) return false;
        if (newItem is Wheel && !(newItem as Wheel).canBeTaken) return false;

        bool isItemAlreadyEquipped = items.Contains(newItem);

        if (!isItemAlreadyEquipped && CurrentSlots != 0) currentSlot = GetSlotToFill();

        //networkPlayer.InventoryScript.CurrentSlots--;

        if (!HasAnyItem(owner))
        {
            SetItemCmd(newItem, owner, isItemAlreadyEquipped);
        }
        else
        {
            Transform oldItemParent = GetItemParent(owner);
            Item oldItem = GetMainItem(owner);

            if (owner.InventoryScript.CurrentSlots > 0)
            {
                currentSlot = GetSlotToFill();

                owner.isBeingTakenToBackpack = true;
                TakeItemToBackpackCmd(oldItem, owner);
                SetItemCmd(newItem, owner, isItemAlreadyEquipped);
            }
            else
            {
                if (oldItem.isCompact)
                {
                    owner.isItemBeingReplaced = true;
                    ReplaceItemCmd(oldItem, newItem);
                }
                else
                {
                    DropItemOnTheGround(owner);
                }

                items[currentSlot] = null;

                SetItemCmd(newItem, owner, isItemAlreadyEquipped);
            }

        }

        if (!isItemAlreadyEquipped)
        {
            items[currentSlot] = newItem;
        }

        UIManager.Instance.uiInventory.SetSlot(newItem, currentSlot, isItemAlreadyEquipped);

        currentItem = newItem;

        return true;
    }

    int GetSlotToFill()
    {
        for (int a = 0; a < items.Count; a++)
        {
            if (items[a] == null) return a;
        }

        return 0;
    }

    bool IsListOnlyNull()
    {
        foreach (Item item in items)
        {
            if (item != null) return false;
        }

        return true;
    }

    bool CheckForUtility(Item newItem, NetworkPlayerController owner)
    {
        if (currentItem is UtilityItem &&
            (currentItem as UtilityItem).objective == newItem.itemName &&
            newItem is IInsertable)
        {
            (newItem as IInsertable).Insert(currentItem as UtilityItem, owner);
            ClearItem(items.IndexOf(currentItem));
            return true;
        }

        return false;
    }

    #region switch item c/s

    [Command(requiresAuthority = false)]
    void SwitchItemCmd(Item item, NetworkPlayerController networkPlayer, int prevSlot)
    {
        SwitchItemRpc(item, networkPlayer, prevSlot);
    }
    [ClientRpc]
    void SwitchItemRpc(Item item, NetworkPlayerController networkPlayer, int prevSlot)
    {
        item.TakeItem(networkPlayer);

        if (networkPlayer.hasAuthority)
        {
            TakeItemClient(item, networkPlayer);
            networkPlayer.isSwitchItemCommandBeingPerformed = false;
        }
    }

    public void SwitchItem(int itemIndex)
    {
        itemIndex -= 1;

        if (items.IndexOf(currentItem) == itemIndex)
        {
            networkPlayer.isSwitchItemCommandBeingPerformed = false;
            return;
        }
        if (itemIndex > items.Count - 1)
        {
            networkPlayer.isSwitchItemCommandBeingPerformed = false; return;
        }
        if (items[itemIndex] == null)
        {
            networkPlayer.isSwitchItemCommandBeingPerformed = false;
            return;
        }
        Prev = currentSlot;
        currentSlot = itemIndex;

        if (HasAnyItem(networkPlayer))
        {
            networkPlayer.isBeingTakenToBackpack = true;
            TakeItemToBackpackCmd(currentItem, networkPlayer);
            SwitchItemCmd(items[itemIndex], networkPlayer, Prev);
        }
        else
        {
            Prev = -1;
            SwitchItemCmd(items[itemIndex], networkPlayer, -1);
        }



        currentItem = items[itemIndex];
    }

    #endregion

    #region set item c/s
    [Command(requiresAuthority = false)]
    void SetItemCmd(Item newItem, NetworkPlayerController owner, bool isItemEquipped)
    {
        SetItem(newItem, owner, isItemEquipped);
    }

    [ClientRpc]
    private void SetItem(Item newItem, NetworkPlayerController owner, bool isItemEquipped)
    {
        if (!isItemEquipped) owner.InventoryScript.CurrentSlots--;
        Transform parentItem = newItem.transform.parent;

        parentItem.SetParent(GetPlayerInventory(owner));

        parentItem.localPosition = newItem.invetoryCustomPosition;
        parentItem.localRotation = Quaternion.Euler(newItem.inventoryCustomRotation);

        parentItem.localScale = newItem.parentScale;

        if (owner.hasAuthority)
        {
            owner.isItemBeingTaken = false;
        }
    }

    #endregion

    public void DropItemOnTheGround(NetworkPlayerController owner)
    {
        if (HasAnyItem(owner))
        {
            Item mainItem = GetMainItem(owner);

            DropItemOnGroundCmd(mainItem, owner);
            items[currentSlot] = null;
            currentItem = null;
            UIManager.Instance.uiInventory.SetSlot(null, currentSlot);
        }

    }


    public Transform GetItemParent(NetworkPlayerController owner)
    {
        if (!HasAnyItem(owner))
        {
            return null;
        }
        else
        {
            return GetPlayerInventory(owner).GetChild(0);
        }
    }

    public Item GetMainItem(NetworkPlayerController owner)
    {
        if (!HasAnyItem(owner))
        {
            return null;
        }
        else
        {
            return GetItemParent(owner).GetChild(0).GetComponent<Item>();
        }
    }

    public Transform GetPlayerInventory(NetworkPlayerController owner)
    {
        return owner.inventory;
    }

    public bool GetMainItemOut(NetworkPlayerController owner, out Item item)
    {
        if (!HasAnyItem(owner))
        {
            item = null;
            return false;
        }
        item = GetMainItem(owner);

        return true;
    }

    public bool GetSearchedItemOut(NetworkPlayerController owner, out Item item, ItemList itemName)
    {
        if (!HasAnyItem(owner))
        {
            item = null;
            return false;
        }
        item = GetMainItem(owner);

        if(item.itemName == itemName){
            return true;
        }
        else{
            item = null;
            return false;
        }
    }

    public bool HasAnyItem(NetworkPlayerController owner)
    {
        return owner.inventory.childCount > 0;
    }

    public void DropOnDeath(NetworkPlayerController owner)
    {
        int index = -1;
        if (currentItem != null) index = items.IndexOf(currentItem);

        DropItemOnTheGround(owner);
        currentItem = null;

        for (int a = 0; a < items.Count; a++)
        {
            if (items[a] != null && a != index)
            {
                DropBackpackItems(items[a], owner, a);
                ClearItem(a);
            }
        }
    }

    public void DropBackpackItems(Item item, NetworkPlayerController owner, int index)
    {
        DropBackPackCmd(item, owner, index);
    }

    [Command(requiresAuthority = false)]
    void DropBackPackCmd(Item item, NetworkPlayerController owner, int index)
    {
        DropBackpackItemsRpc(item, owner, index);
    }

    [ClientRpc]
    void DropBackpackItemsRpc(Item item, NetworkPlayerController owner, int index)
    {
        Transform _itemParent = item.transform.parent;

        Ray _ray = new Ray(owner.dropPosition.position, -Vector3.up);
        Physics.Raycast(_ray, out _raycastHit);

        _itemParent.SetParent(null);
        _itemParent.position = _raycastHit.point;
        _itemParent.localRotation = Quaternion.Euler(Vector3.zero);
        _itemParent.localScale = item.parentScale;
    }

    public void ClearItem(int index, bool increaesSlot = true)
    {
        UIManager.Instance.uiInventory.SetSlot(null, index);
        if (increaesSlot)
            IncreaseSlotsCmd(networkPlayer);

        if (currentItem == items[index])
        {
            currentItem = null;
        }

        items[index] = null;

    }

    [Command(requiresAuthority = false)]
    void IncreaseSlotsCmd(NetworkPlayerController networkPlayer)
    {
        IncrealeSlotRpc(networkPlayer);
    }
    [ClientRpc]
    void IncrealeSlotRpc(NetworkPlayerController networkPlayer)
    {
        networkPlayer.InventoryScript.CurrentSlots++;
    }

    [Command(requiresAuthority = false)]
    void DecreaseSlotCmd(NetworkPlayerController networkPlayer)
    {
        CurrentSlots--;
    }

    #region backpack item c/s
    [Command(requiresAuthority = false)]
    void TakeItemToBackpackCmd(Item oldItem, NetworkPlayerController owner)
    {
        owner.InventoryScript.CurrentSlots--;
        TakeItemToBackpackRpc(oldItem, owner);
    }
    [ClientRpc]
    void TakeItemToBackpackRpc(Item oldItem, NetworkPlayerController owner)
    {
        Transform itemParent = oldItem.transform.parent;
        (oldItem as ITakeable).OnDropItem();

        itemParent.SetParent(owner.Backpack, true);
        itemParent.localPosition = Vector3.zero;
        itemParent.localRotation = Quaternion.identity;

        if (owner.hasAuthority)
        {
            owner.isBeingTakenToBackpack = false;
        }

    }
    #endregion

    #region replace item c/s
    [Command(requiresAuthority = false)]
    void ReplaceItemCmd(Item oldItem, Item newItem)
    {
        ReplaceItemRpc(oldItem, newItem);
    }
    [ClientRpc]
    void ReplaceItemRpc(Item oldItem, Item newItem)
    {
        Transform oldItemParent = oldItem.transform.parent;

        (oldItem as ITakeable).OnDropItem();

        oldItemParent.SetParent(null);
        oldItemParent.position = newItem.transform.parent.position;
        oldItemParent.localRotation = newItem.transform.parent.rotation;

        networkPlayer.isItemBeingReplaced = false;

    }
    #endregion

    #region drop item c/s
    [Command(requiresAuthority = false)]
    void DropItemOnGroundCmd(Item droppedItem, NetworkPlayerController owner)
    {
        DropItemOnGroundRpc(droppedItem, owner);
    }
    [ClientRpc]
    void DropItemOnGroundRpc(Item droppedItem, NetworkPlayerController owner)
    {
        Transform _itemParent = droppedItem.transform.parent;
        droppedItem.OnDropItem();

        Ray _ray = new Ray(owner.dropPosition.position, -Vector3.up);
        Physics.Raycast(_ray, out _raycastHit);

        _itemParent.SetParent(null);
        _itemParent.position = _raycastHit.point;
        _itemParent.localRotation = Quaternion.Euler(Vector3.zero);

        if (owner.hasAuthority)
        {
            owner.isDropCommandBeingPerformed = false;
        }
    }
    #endregion

    public void OnItemCombineHandler(UtilityItem util, IInsertable ins)
    {
        ins.Insert(util, networkPlayer);
        ClearItem(items.IndexOf(util as Item));

        UIManager.Instance.PlaySound(util.onInsertClip);
    }

    public void CheckCompatibility(UtilityItem util, IInsertable ins)
    {
        Debug.Log("happened");
    }

}

