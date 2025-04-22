using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using chloeprice;
using UnityEngine.EventSystems;

public class InventoryBackpackSlot : InventorySlot
{
    [SerializeField] Animation appearAnim;
    public TextMeshProUGUI labelTF;

    [SerializeField] EventTrigger eventTrigger;

    UiInventory inventory;
    public bool canTriggerOnLeave;

    private void Start()
    {
        inventory = UIManager.Instance.uiInventory;
    }

    private void OnEnable()
    {
        canTriggerOnLeave = true;
        appearAnim["QuickSlotAppear"].speed = 1;
        appearAnim.Play();
    }

    public void OnHideSlot()
    {
        canTriggerOnLeave = false;
        My.PlayReversedAnim(appearAnim, "QuickSlotAppear");
    }

    public void _OnMouseEnter()
    {
        UIManager.Instance.PlaySound(inventory.slotHoverSound, 1.5f);
        inventory._hoveredBPSlot = this;

        if (item is null)
        {
            inventory.descriptionText.text = string.Empty;
            inventory.titleText.text = string.Empty;
        }
        else
        {
            inventory.descriptionText.text =
                LocalizationSettings.StringDatabase.GetLocalizedString("itemDescription", item.inspectDescription);

            inventory.titleText.text =
                LocalizationSettings.StringDatabase.GetLocalizedString("itemDescriptionTitles", item.inspectLabel);
        }



        if (inventory.isBeingDragged && inventory.CheckCompatibility(inventory._draggedItem, inventory._hoveredBPSlot.item))
        {
            Debug.Log("compatible");
            border.color = Color.green;
        }
        else if (inventory.isBeingDragged)
        {
            border.color = Color.red;
        }

        appearAnim["BackpackSlotOnMouseHover"].speed = 1;
        appearAnim.PlayQueued("BackpackSlotOnMouseHover");
    }

    public void _OnMouseLeave()
    {
        inventory._hoveredBPSlot = null;
        border.color = Color.white;
        if (!canTriggerOnLeave) return;
        My.PlayReversedAnim(appearAnim, "BackpackSlotOnMouseHover");
    }

    public void _OnMouseDrag()
    {
        if (item == null) return;
        inventory._draggedItem = item;
        inventory.SetDraggedImage(item.iconSprite);

    }

    public void _OnMouseDragStop()
    {
        Debug.Log("MOUSE DRAG STOP" + NetworkPlayerController.NetworkPlayer.name);
        if (item == null || inventory._hoveredBPSlot?.item == null)
        {
            inventory.HideDraggedImage();
            return;
        }

        Debug.Log($"hovering {inventory._hoveredBPSlot.item} and its " +
    $"item is {inventory._hoveredBPSlot.item}");

        if (inventory.CheckCompatibility(item,
            inventory._hoveredBPSlot.item,
            true))
        {
            Debug.Log("compatible");
        }
        else
        {
            Debug.Log("the're not compatible");
        }

        border.color = Color.white;

        inventory.HideDraggedImage();
    }

    public void _OnContiniousDrag(BaseEventData data)
    {
        PointerEventData newd = (PointerEventData)data;
        inventory.draggedImage.rectTransform.parent.position = new Vector3(newd.position.x - 40, newd.position.y, 0);
    }

    public override void Set(Item item)
    {
        base.Set(item);
        labelTF.text = item?.inspectLabel ?? string.Empty;



    }
}
