using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Screwdriver : Tool
{
    private float _timeElapsed = 0f;


    public override void OperatePerformed(InputAction.CallbackContext context)
    {
        _timeElapsed += Time.deltaTime;
        _isHold = true;

        if (_owner != null)
        {
            _owner.switchInput.Disable();
        }

        StartCoroutine(ScrewCoroutine());
    }

    public override void OperateCanceled(InputAction.CallbackContext context)
    {

        if (_owner != null)
        {
            _owner.switchInput.Enable();
        }
        _isHold = false;
        CmdEndScrewProccess();
    }
    

    IEnumerator ScrewCoroutine()
    {
        Screw _screw = null;

        _ray = _ownerCopy.playerCamera.ViewportPointToRay(new Vector3(.5f, .5f, 0));
        _didHit = Physics.Raycast(_ray, out _impactedObject, PlayerAttributes.interactRange, _keyObjectLayerMask);

        if (_didHit && _impactedObject.collider.CompareTag(targetObjectTag))
        {
            _screw = _impactedObject.collider.GetComponent<Screw>();
            CmdScrewProccessStart();
        }

        while (_isHold && _didHit && _impactedObject.collider.CompareTag(targetObjectTag))
        {
            if (!_operateAudioSource.isPlaying)
            {
                CmdScrewProccessStart();
            }

            _ray = _ownerCopy.playerCamera.ViewportPointToRay(new Vector3(.5f, .5f, 0));

            if (!Physics.Raycast(_ray, out _impactedObject, PlayerAttributes.interactRange, _keyObjectLayerMask))
            {
                break;
            }
            if (_impactedObject.collider.gameObject.tag != targetObjectTag)
            {
                break;
            }

            _screw.Unscrew();

            transform.Rotate(0, -.5f, 0);

            yield return null;
        }
    }


    [Command(requiresAuthority = false)]
    void CmdScrewProccessStart()
    {
        RpcScrewProcessStart();
    }
    [ClientRpc]
    void RpcScrewProcessStart()
    {
        _operateAudioSource.Play();
    }

    [Command(requiresAuthority = false)]
    void CmdEndScrewProccess()
    {
        RpcEndScrewProcess();
    }
    [ClientRpc]
    void RpcEndScrewProcess()
    {
        if (_operateAudioSource.isPlaying)
        {
            _operateAudioSource.Stop();
        }
    }
}
