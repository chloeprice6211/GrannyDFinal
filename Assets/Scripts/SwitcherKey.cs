using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitcherKey : Item
{
    private Ray _ray;
    private bool _didHit;
    private RaycastHit _impactedObject;
    private bool _isShown;

    private NetworkPlayerController _ownerCopy;
    public float interactRange = 2;
    [SerializeField] LayerMask layerMask;

    public override void OnDropItem()
    {
        base.OnDropItem();
        StopAllCoroutines();
    }

    public override void TakeItem(NetworkPlayerController owner)
    {
       
        base.TakeItem(owner);
        if (!owner.hasAuthority) return;
        _ownerCopy = owner;
        

        StartCoroutine(RayCoroutine());

    }

    IEnumerator RayCoroutine()
    {
        while (true)
        {
            _ray = _ownerCopy.playerCamera.ViewportPointToRay(new Vector3(.5f, .5f, 0));
            _didHit = Physics.Raycast(_ray, out _impactedObject, interactRange, layerMask);

            if (_didHit)
            {
                if (_impactedObject.collider.gameObject.name == "KeyPanel")
                {
                    UIManager.Instance.ShowInteractOption(UIManager.Instance.UIRayToolText[3]);
                    _isShown = true;
                }
            }
            else if (_isShown == true)
            {
                _isShown = false;
                UIManager.Instance.HideInteractOption();
            }


            yield return null;
        }
    }
}
