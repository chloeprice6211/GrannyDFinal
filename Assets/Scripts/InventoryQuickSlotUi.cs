using chloeprice;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryQuickSlotUi : InventorySlot
{
    [SerializeField] Animation qsAnimation;

    [SerializeField] AnimationClip onSelectAnimClip;
    [SerializeField] AnimationClip appearAnimClip;

    public bool IsActive;

    private void OnEnable()
    {
        qsAnimation.Play(appearAnimClip.name);
    }

    public void OnSelect()
    {
        IsActive = true;
        qsAnimation[onSelectAnimClip.name].speed = 1;
        qsAnimation.Play(onSelectAnimClip.name);
    }
    public void OnDiselect()
    {
        IsActive = false;
        My.PlayReversedAnim(qsAnimation, onSelectAnimClip.name);
    }
}
