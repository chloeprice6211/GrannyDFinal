using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public Image icon;
    public Image border;
    public Item item;

    public virtual void Set(Item _item)
    {
        item = _item;
        icon.sprite = _item?.iconSprite ?? UIManager.Instance.uiInventory.nullSprite;
        //border.color = Color.green;
    }

    public void SetAsCurrent()
    {

    }
}
