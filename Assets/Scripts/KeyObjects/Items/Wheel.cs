using Mirror;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public interface IInstallable
{
    public void OnInstall();
}

public interface IOperable
{
    public bool Operate();
}

public class Wheel : Tool, IInstallable, IOperable
{
    public bool isInstalled;
    public float wheelInstalationTime;
    public bool canBeTaken = true;

    [SerializeField] Car car;
    [SerializeField] AudioClip installSound;

    private float _timeElapsed;

    public override void TakeItem(NetworkPlayerController owner)
    {
        if (!canBeTaken) return;
        base.TakeItem(owner);
    }


    [Command (requiresAuthority = false)]
    public void CompleteWheelInstall()
    {
        car.hasWheel = true;
        RpcCompleteWheelInstall();
    }
    [ClientRpc]
    void RpcCompleteWheelInstall()
    {
        Destroy(_collider);
    }

    public void OnInstall()
    {
        AudioSource.PlayClipAtPoint(installSound, transform.position);
        tag = "Empty";
        canBeTaken = false;
    }

    public bool Operate()
    {
        _timeElapsed += Time.deltaTime;

        if (_timeElapsed >= wheelInstalationTime)
        {
            CompleteWheelInstall();

            return true;
        }

        return false;
    }

    public override void OperateCanceled(InputAction.CallbackContext obj)
    {
        
    }

    public override void OperatePerformed(InputAction.CallbackContext obj)
    {
        
    }
}
