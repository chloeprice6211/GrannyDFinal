using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wrench : Tool
{
    [SerializeField] Wheel wheel;

    [SerializeField] Animation _animation;
    [SerializeField] AnimationClip operatingClip;


    //input controls
    public override void OperateCanceled(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (_owner != null)
        {
            _owner.switchInput.Enable();
        }

        _isHold = false;
        CmdEndOperating();
    }

    public override void OperatePerformed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
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

        _ray = _ownerCopy.playerCamera.ViewportPointToRay(new Vector3(.5f, .5f, 0));
        _didHit = Physics.Raycast(_ray, out _impactedObject, PlayerAttributes.interactRange, _keyObjectLayerMask);
        _animation.Play(operatingClip.name);


        while (_isHold && _didHit)
        {
            if (!wheel.isInstalled) break;

            _ray = _ownerCopy.playerCamera.ViewportPointToRay(new Vector3(.5f, .5f, 0));

            if (!Physics.Raycast(_ray, out _impactedObject, PlayerAttributes.interactRange, _keyObjectLayerMask))
            {
                break;
            }
            if (_impactedObject.collider.gameObject.name != targetObjectTag)
            {
                break;
            }

            if (!_operateAudioSource.isPlaying)
            {
                CmdStartOperating();
            }

            if (wheel.Operate())
            {
                CmdEndOperating();
                break;
            }

            yield return null;
        }

        _animation.Stop();

        transform.localRotation = Quaternion.Euler(new Vector3(0,0,-90));

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
    void CmdEndOperating()
    {
        RpcEndOperating();
    }
    [ClientRpc]
    void RpcEndOperating()
    {
        _operateAudioSource.Stop();
    }

}
