using Mirror;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class JerryCan : Tool
{
    [SerializeField] Animation _animation;

    [SerializeField] AnimationClip pourForward;
    [SerializeField] AnimationClip pourBackward;

    [SerializeField] FuelTankNew fuelTank;

    public override void OperateCanceled(InputAction.CallbackContext obj)
    {
        _isHold = false;
        if (_owner != null)
        {
            _owner.switchInput.Enable();
        }
    }

    public override void OperatePerformed(InputAction.CallbackContext obj)
    {
        if (_owner != null)
        {
            _owner.switchInput.Disable();
        }
        _isHold = true;
        StartCoroutine(OperateCoroutine());
    }

    IEnumerator OperateCoroutine()
    {
        bool hasAnimationPlayed = false;

        _ray = _ownerCopy.playerCamera.ViewportPointToRay(new Vector3(.5f, .5f, 0));
        _didHit = Physics.Raycast(_ray, out _impactedObject, PlayerAttributes.interactRange, _keyObjectLayerMask);


        while (_isHold && _didHit)
        {
            _ray = _ownerCopy.playerCamera.ViewportPointToRay(new Vector3(.5f, .5f, 0));

            if (!Physics.Raycast(_ray, out _impactedObject, PlayerAttributes.interactRange, _keyObjectLayerMask))
            {
                _operateAudioSource.Pause();
                break;
            }
            if (_impactedObject.collider.gameObject.name != targetObjectTag)
            {
                _operateAudioSource.Pause();
                break;
            }

            if (!_operateAudioSource.isPlaying)
            {
                CmdStartOperating();
            }

            if (!hasAnimationPlayed)
            {
                _animation.Play(pourForward.name);
                hasAnimationPlayed = true;
            }

            if (fuelTank.FuelCar())
            {
                break;
            }

            yield return null;
        }

        if (hasAnimationPlayed)
        {
            _animation.Play(pourBackward.name);
        }

        CmdStopOperating();

    }

    [Command(requiresAuthority = false)]
    void CmdStartOperating()
    {
        RpcStartOperating();
    }
    [ClientRpc]
    void RpcStartOperating()
    {
        _operateAudioSource.Play();
    }

    [Command(requiresAuthority = false)]
    void CmdStopOperating()
    {
        RpcStopOperating();
    }
    [ClientRpc]
    void RpcStopOperating()
    {
        _operateAudioSource.Pause();
    }

}
