using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class Tool : Item, IHasControls
{
    //interaction
    public string targetObjectTag;
    public string interactMessage;
    public int keyIndex;
    public bool requireHold;

    private const string extraKeyWord = "<b><color=#0affae> [hold]</color></b>";

    //raycast
    protected Ray _ray;
    protected bool _didHit;
    protected RaycastHit _impactedObject;
    protected bool _isShown;
    protected bool _canCastRay = true;
    [SerializeField] protected LayerMask _keyObjectLayerMask;
    public bool canCastCustomRay = true;

    protected NetworkPlayerController _ownerCopy;

    //controls
    private PlayerControls _toolControls;
    private InputAction _operateAction;
    [SyncVar] protected bool _isHold;

    //public
    [SerializeField] protected AudioSource _operateAudioSource;


    public override void Awake()
    {
        base.Awake();

        _toolControls = new();
        _operateAction = _toolControls.Tool.Operate;

        _operateAction.performed += OperatePerformed;
        _operateAction.canceled += OperateCanceled;

    }

    public abstract void OperateCanceled(InputAction.CallbackContext obj);

    public abstract void OperatePerformed(InputAction.CallbackContext obj);


    public override void TakeItem(NetworkPlayerController owner)
    {
        base.TakeItem(owner);

        if (!owner.hasAuthority) return;

        _ownerCopy = owner.GetComponent<NetworkPlayerController>();

        ReturnControls();
        StartCoroutine(RayCoroutine());

    }

    public virtual IEnumerator RayCoroutine()
    {
        while (true)
        {
            _ray = _ownerCopy.playerCamera.ViewportPointToRay(new Vector3(.5f, .5f, 0));
            _didHit = Physics.Raycast(_ray, out _impactedObject, PlayerAttributes.interactRange, _keyObjectLayerMask);

            if (!canCastCustomRay) break;

            if (_didHit)
            {
                if (_impactedObject.collider.gameObject.name == targetObjectTag)
                {
                    if (requireHold)
                    {
                        UIManager.Instance.ShowInteractOption(UIManager.Instance.UIRayToolText[keyIndex] + extraKeyWord);
                    }
                    else
                    {
                        UIManager.Instance.ShowInteractOption(UIManager.Instance.UIRayToolText[keyIndex]);
                    }

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

    public override void OnDropItem()
    {
        if (_owner != null) _owner.switchInput.Enable(); 
        base.OnDropItem();
        DisableItemControls();
        StopAllCoroutines();

    }

    public void DisableItemControls()
    {
        _operateAction.Disable();
    }

    public void ReturnControls()
    {
        _operateAction.Enable();
    }
}


