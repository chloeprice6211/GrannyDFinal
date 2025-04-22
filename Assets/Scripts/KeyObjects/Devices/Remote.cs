using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;

public class Remote : Device, IInsertable
{
    [SerializeField] Material inactiveMaterial;
    [SerializeField] Material unlockedMaterial;
    [SerializeField] Material lockedMaterial;
    [SerializeField] AudioClip switchSound;
    [SerializeField] AudioClip batteryInstertSound;
    [SerializeField] Renderer objectRenderer;
    [SerializeField] Light lightSource;
    [SerializeField] GarageIndicator garageController;

    [SyncVar] public bool isGarageUnlocked;
    public bool isOn;
    public bool isPowered;
    [SyncVar] public bool canOpen = true;

    public void TurnRemoteOn()
    {
        isOn = true;
        lightSource.enabled = true;

        AudioSource.PlayClipAtPoint(switchSound, transform.position);

        if (isGarageUnlocked)
        {
            objectRenderer.material = unlockedMaterial;
        }
        else
        {
            objectRenderer.material = lockedMaterial;
        }
    }

    public void TurnRemoteOff()
    {
        AudioSource.PlayClipAtPoint(switchSound, transform.position);

        objectRenderer.material = inactiveMaterial;
        isOn = false;
        lightSource.enabled = false;
    }

    #region Open gate c/s

    [Command(requiresAuthority = false)]
    public void OpenGateCommand()
    {
        OpenGateRpc();
    }
    [ClientRpc]
    private void OpenGateRpc()
    {
        TurnRemoteOn();
        garageController.OpenGate();
        canOpen = false;
    }

    public void OpenGate()
    {
        AudioSource.PlayClipAtPoint(switchSound, transform.position);

        if (!isPowered)
        {
            UIManager.Instance.Message("remoteBattery", "remoteBattery_A");
            return;
        }

        if (!isGarageUnlocked)
        {
            UIManager.Instance.Message("gatesUnlock", "unlockGateFirst_A");
            return;
        }


        if (!canOpen || !isGarageUnlocked || !isPowered)
        {
            return;
        }

        OpenGateCommand();
       
        UIManager.Instance.Message("gatesWontOpen", "gatesWontOpen_A");
    }

    #endregion

    #region Unlock gate c/s

    [Command(requiresAuthority = false)]
    private void UnlockGateCommand()
    {
        UnlockGateRpc();
    }
    [ClientRpc]
    private void UnlockGateRpc()
    {
        if (!garageController.Unlock()) return;
        AudioSource.PlayClipAtPoint(switchSound, transform.position);
        isGarageUnlocked = true;
        TurnRemoteOn();
    }

    public void UnlockGate()
    {
        if (!isPowered)
        {
            UIManager.Instance.Message("remoteBattery", "remoteBattery_A");
            return;
        }

        UnlockGateCommand();
    }

    #endregion

    #region Lock gate c/s

    [Command(requiresAuthority = false)]
    private void LockGateCommand()
    {
        LockGateRpc();
    }
    [ClientRpc]
    private void LockGateRpc()
    {
        if (!garageController.Lock()) return;
        AudioSource.PlayClipAtPoint(switchSound, transform.position);
        isGarageUnlocked = false;
        TurnRemoteOn();
    }

    public void LockGate()
    {
        if (!isPowered)
        {
            UIManager.Instance.Message("remoteBattery", "remoteBattery_A");
            return;
        }

        LockGateCommand();
    }

    #endregion

    #region enable remote c/s

    void EnableRemote()
    {
        AudioSource.PlayClipAtPoint(switchSound, transform.position);
        if (!isPowered) return;

        CmdEnableRemote();
    }

    [Command(requiresAuthority = false)]
    private void CmdEnableRemote()
    {
        RpcEnableRemote();
    }
    [ClientRpc]
    private void RpcEnableRemote()
    {
        if (isOn)
        {
            TurnRemoteOff();
        }
        else
        {
            TurnRemoteOn();
        }
    }

    #endregion

    //abstract class implementation
    public override void TakeItem(NetworkPlayerController owner)
    {
        base.TakeItem(owner);
    }

    public void Insert(UtilityItem itemToInsert, NetworkPlayerController owner)
    {
        UIManager.Instance.Message("remoteInsertBattery", "shouldWorkV2_A");
        InsertCmd(itemToInsert);
    }
    [Command(requiresAuthority = false)]
    void InsertCmd(UtilityItem itemToInsert)
    {
        InsertRpc(itemToInsert);
    }
    [ClientRpc]
    void InsertRpc(UtilityItem item)
    {
        isPowered = true;
        AudioSource.PlayClipAtPoint(batteryInstertSound, transform.position);
        Destroy(item.transform.parent.gameObject);
    }

    #region On button clicks

    public void OnEnableClick()
    {
        EnableRemote();
    }

    public void OnUnlockClick()
    {
        UnlockGate();
    }

    public void OnLockClick()
    {
        LockGate();
    }

    public void OnOpenGateClick()
    {
        OpenGate();
    }

    #endregion

}
