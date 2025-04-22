using chloeprice;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class UiInventory : MonoBehaviour
{
    [SerializeField] Animation inventoryAnimation;

    public Sprite nullSprite;
    [SerializeField] GameObject backpackObj;
    [SerializeField] GameObject quickSlotsObj;

    public Image draggedImage;

    public delegate void OnItemCombine(UtilityItem item, IInsertable ins);


    [SerializeField] List<InventoryBackpackSlot> inventoryBackpackSlots;
    [SerializeField] List<InventoryQuickSlotUi> inventoryQuickSlots;

    public InventoryBackpackSlot _hoveredBPSlot;
    public bool isBeingDragged;
    public Item _draggedItem;

    [SerializeField] AudioClip backpackSoundOpen;

    public AudioClip slotHoverSound;

    public bool IsActive;

    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    NetworkPlayerController _owner;

    public void ShowOrHide(NetworkPlayerController owner)
    {
        _owner = owner;
        if (inventoryAnimation.isPlaying) return;
        if (!backpackObj.activeInHierarchy)
        {
            if (owner.isRpcBeingProcessed) return;
            Show();
        }
        else
        {
            Close();
        }
    }
    public void Show()
    {
        IsActive = true;

        _owner.interactInput.Disable();
        _owner._cameraController.canMove = false;
        _owner.pauseInput.Disable();
        _owner.dropInput.Disable();

        UIManager.Instance.PlaySound(backpackSoundOpen);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        titleText.text = string.Empty;
        descriptionText.text = string.Empty;

        CancelInvoke(nameof(CloseDelayed));
        backpackObj.SetActive(true);
        inventoryAnimation["inventoryIn"].speed = 1;
        inventoryAnimation.Play("inventoryIn");
    }

    public void Close(bool enableControls = true)
    {
        IsActive = false;

        if (enableControls)
        {
            _owner.pauseInput.Enable();
            _owner.interactInput.Enable();
            _owner._cameraController.canMove = true;
            _owner.dropInput.Enable();
        }
        


        foreach (InventoryBackpackSlot slot in inventoryBackpackSlots)
        {
            slot.OnHideSlot();
        }

        My.PlayReversedAnim(inventoryAnimation, "inventoryIn");
        HideDraggedImage();

        Cursor.lockState = CursorLockMode.Locked;


        Invoke(nameof(CloseDelayed), inventoryAnimation["inventoryIn"].length);
    }

    public void SetDraggedImage(Sprite icon)
    {
        draggedImage.sprite = icon;
        draggedImage.rectTransform.parent.localScale = Vector3.one;
        isBeingDragged = true;
    }

    public void HideDraggedImage()
    {
        draggedImage.sprite = null;
        draggedImage.rectTransform.parent.localScale = Vector3.zero;
        isBeingDragged = false;
        _draggedItem = null;
    }

    public void OnBackpackPickup(int slots)
    {
        for (int a = 2; a < slots + 2; a++)
        {
            inventoryBackpackSlots[a].gameObject.SetActive(true);
            inventoryQuickSlots[a].gameObject.SetActive(true);
        }
    }

    public void SetSlot(Item item, int index, bool isAlreadyEquipped = false)
    {
        Debug.Log(index);
        inventoryQuickSlots[index].Set(item);
        inventoryBackpackSlots[index].Set(item);

        if (!isAlreadyEquipped)
        {
            //UtilityItem util = LookForUtility();
            //IInsertable ins = LookForInsertable();

            //if (CheckCompatibility(util, ins))
            //{
            //    Debug.Log($"{util.name} and {(ins as Item).name} are compatible");
            //}
        }

        for (int a = 0; a < inventoryQuickSlots.Count; a++)
        {
            if (inventoryQuickSlots[a].IsActive) inventoryQuickSlots[a].OnDiselect();
        }

        if (item != null)
        {
            inventoryQuickSlots[index].OnSelect();
        }

        inventoryBackpackSlots[index].labelTF.text = item is not null ?
            LocalizationSettings.StringDatabase.GetLocalizedString("itemDescriptionTitles", item.inspectLabel) :
            string.Empty;

        titleText.text = inventoryBackpackSlots[index].labelTF.text;

        descriptionText.text = item is not null ?
            LocalizationSettings.StringDatabase.GetLocalizedString("itemDescription", item.inspectDescription) :
            string.Empty;

    }

    public void HideQuickSlots()
    {
        quickSlotsObj.SetActive(false);
    }

    public void ShowQuickSlots()
    {
        quickSlotsObj.SetActive(true);
    }

    void CloseDelayed()
    {
        backpackObj.SetActive(false);
    }

    UtilityItem LookForUtility()
    {
        for (int a = 0; a < inventoryBackpackSlots.Count; a++)
        {
            if (inventoryBackpackSlots[a].item is UtilityItem)
            {
                return inventoryBackpackSlots[a].item as UtilityItem;
            }
        }

        return null;
    }

    IInsertable LookForInsertable()
    {
        for (int a = 0; a < inventoryBackpackSlots.Count; a++)
        {
            if (inventoryBackpackSlots[a].item is IInsertable)
            {
                return inventoryBackpackSlots[a].item as IInsertable;
            }
        }

        return null;
    }

    public bool CheckCompatibility(Item item1, Item item2, bool invokeEvent = false)
    {
        if (item1 == null || item2 == null) return false;

        if (item1 is IInsertable && item2 is UtilityItem)
        {
            if ((item2 as UtilityItem).objective == item1.itemName)
            {
                if (invokeEvent)
                {
                    NetworkPlayerController.NetworkPlayer.InventoryScript.OnItemCombineHandler(item2 as UtilityItem, item1 as IInsertable);
                   
                }
                return true;
            }
        }
        else if (item1 is UtilityItem && item2 is IInsertable)
        {
            if ((item1 as UtilityItem).objective == item2.itemName)
            {
                if (invokeEvent)
                {
                    NetworkPlayerController.NetworkPlayer.InventoryScript.OnItemCombineHandler(item1 as UtilityItem, item2 as IInsertable);
                   
                }
                return true;
            }
        }

        Debug.Log("not compatible" + item1.name + item2.name);
        return false;
    }
}
