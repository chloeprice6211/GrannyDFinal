using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Unity.VisualScripting;
using UnityEngine.InputSystem.XR;

public class CarJack : Tool
{
    public float carJackOperationTime = 5f;

    private bool _isCompleted;
    private float _timeElapsed = 0f;
    [SyncVar] public bool isInstalled = false;
    [SerializeField] Animation _animation;
    private bool _hasBeenOperated;

    [SerializeField] AnimationClip operatingAnimation;
    [SerializeField] AnimationClip uninstallAnimation;
    [SerializeField] AudioClip uninstall;
    [SerializeField] CarFixArea carFixArea;


    public override void OperateCanceled(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        _isHold = false;
        if (_owner != null)
        {
            _owner.switchInput.Enable();
        }
        CmdStopOperating(_ownerCopy);
    }

    public override void OperatePerformed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        _isHold = true;
        if (_owner != null)
        {
            _owner.switchInput.Disable();
        }
        StartCoroutine(OperateCoroutine());
    }


    //interface implementation
    public override void TakeItem(NetworkPlayerController item)
    {
        if (isInstalled)
        {
            AudioSource.PlayClipAtPoint(uninstall, transform.position);
            carFixArea.isCarJackInstalled = false;
            _timeElapsed = 0f;

            isInstalled = false;

        }
        base.TakeItem(item);
    }


    IEnumerator OperateCoroutine()
    {
        _ray = _ownerCopy.playerCamera.ViewportPointToRay(new Vector3(.5f, .5f, 0));
        _didHit = Physics.Raycast(_ray, out _impactedObject, PlayerAttributes.interactRange, _keyObjectLayerMask);


        while (_isHold && _didHit)
        {
            _ray = _ownerCopy.playerCamera.ViewportPointToRay(new Vector3(.5f, .5f, 0));

            Physics.Raycast(_ray, out _impactedObject, PlayerAttributes.interactRange, _keyObjectLayerMask);

            if (!Physics.Raycast(_ray, out _impactedObject, PlayerAttributes.interactRange, _keyObjectLayerMask))
            {
                _operateAudioSource.Stop();
                break;
            }
            if (_impactedObject.collider.gameObject.name != targetObjectTag)
            {
                _operateAudioSource.Stop();
                break;
            }

            if (!_animation.isPlaying)
            {
                CmdStartOperating();
            }

            _hasBeenOperated = true;

            _timeElapsed += Time.deltaTime;

            if (_timeElapsed >= carJackOperationTime)
            {
                UIManager.Instance.HideInteractOption();
                _ownerCopy.InventoryScript.ClearItem(_ownerCopy.InventoryScript.items.IndexOf(this), false);
                CmdCompleteSetup();
                break;
            }

            yield return null;
        }

        if (_hasBeenOperated)
        {
            //if(isInstalled) OnDropItem();
            UIManager.Instance.HideInteractOption();
            CmdInstall(_ownerCopy);
        }


        _hasBeenOperated = false;

    }

    [Command(requiresAuthority = false)]
    void CmdStartOperating()
    {
        RpcStartOperating();
    }
    [ClientRpc]
    void RpcStartOperating()
    {
        Debug.Log("start");

        transform.parent.SetParent(carFixArea.carJackPos);
        transform.parent.localPosition = Vector3.zero;
        transform.parent.localRotation = Quaternion.identity;

        _animation.Play(operatingAnimation.name);

        if (!_operateAudioSource.isPlaying)
        {
            _operateAudioSource.Play();
        }

    }

    [Command(requiresAuthority = false)]
    void CmdStopOperating(NetworkPlayerController _controller)
    {
        RpcStopOperating(_controller);
    }
    [ClientRpc]
    void RpcStopOperating(NetworkPlayerController _controller)
    {
        _operateAudioSource.Stop();
        _animation.Stop();

    }

    [Command(requiresAuthority = false)]
    void CmdCompleteSetup()
    {
        isInstalled = true;
    }

    [Command(requiresAuthority = false)]
    void CmdInstall(NetworkPlayerController _controller)
    {
        if (isInstalled)
        {
            RpcInstall(_controller);
        }
        else
        {
            RpcReturnToOwner(_controller);
        }
    }
    [ClientRpc]
    void RpcInstall(NetworkPlayerController controller)
    {
        carFixArea.isCarJackInstalled = true;
        transform.parent.SetParent(null);
        OnDropItem();

        if (controller.hasAuthority)
            UIManager.Instance.HideInteractOption();
    }
    [ClientRpc]
    void RpcReturnToOwner(NetworkPlayerController _controller)
    {
        transform.parent.SetParent(Inventory.Instance.GetPlayerInventory(_controller));
        transform.parent.localPosition = invetoryCustomPosition;
        transform.parent.localRotation = Quaternion.Euler(inventoryCustomRotation);
        _timeElapsed = 0f;
    }

}
