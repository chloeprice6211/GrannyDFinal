using System.Collections;
using UnityEngine;

public class Fuse : Item
{
    private Ray _ray;
    private bool _didHit;
    private RaycastHit _impactedObject;
    private bool _isShown;

    private NetworkPlayerController _ownerCopy;
    public float interactRange = 2;
    public string label;
    [SerializeField] LayerMask layerMask;
    public Animation tubeAnimation;
    public override void OnDropItem()
    {
        base.OnDropItem();

        if(_impactedObject.collider.gameObject.name == label){
            UIManager.Instance.HideInteractOption();
        }


        StopAllCoroutines();


    }

    public override void TakeItem(NetworkPlayerController owner)
    {
        gameObject.layer = 0;
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
                if (_impactedObject.collider.gameObject.name == label)
                {
                    UIManager.Instance.ShowInteractOption(UIManager.Instance.UIRayToolText[4]);
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
