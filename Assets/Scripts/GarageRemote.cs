using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Net.Cache;
using UnityEngine;

public class GarageRemote : Device, IInsertable
{
    [Header("materials/renderer")]
    [SerializeField] Material inactiveMaterial;
    [SerializeField] Material unlockedMaterial;
    [SerializeField] Material lockedMaterial;
    [SerializeField] Renderer objectRenderer;

    [Header("audio")]
    [SerializeField] AudioClip switchSound;
    [SerializeField] AudioClip batteryInstertSound;

    [Header("sync vars")]
    public bool isOn;
    public bool isPowered;

    [Header("misc")]
    [SerializeField] Light lightSource;
    [SerializeField] GarageController garageController;


    public void TurnRemoteOn()
    {
        isOn = true;
        lightSource.enabled = true;

        AudioSource.PlayClipAtPoint(switchSound, transform.position);

        if (garageController.isUnlocked)
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
    }

    public void OpenGate()
    {
        AudioSource.PlayClipAtPoint(switchSound, transform.position);

        if (!isPowered)
        {
            UIManager.Instance.Message("remoteBattery", "remoteBattery_A");
            return;
        }
        if (!garageController.isUnlocked)
        {
            UIManager.Instance.Message("gatesUnlock", "unlockGateFirst_A");
            return;
        }
        if (!garageController.isPowered)
        {
            UIManager.Instance.Message("powerlessGate", "powerlessGate_A");
            return;
        }

        OpenGateCommand();
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
        if (garageController.isUnlocked) return;
        AudioSource.PlayClipAtPoint(switchSound, transform.position);
        garageController.UnlockGate();
        TurnRemoteOn();
        garageController.OpenGate();
        UIManager.Instance.Message("gatesSuccess", "gatesOpened_A");
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
        if (!garageController.isUnlocked) return;

        AudioSource.PlayClipAtPoint(switchSound, transform.position);
        garageController.LockGate();
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
        //if (!isPowered)
        //{
        //    Item item = Inventory.Instance.GetMainItem(owner);

        //    if (item != null && item is Battery)
        //    {
        //        //if (owner.hasAuthority)
        //        //{
        //        //    UIManager.Instance.Message("remoteInsertBattery", "shouldWorkV2_A");
        //        //}

        //        //if (ObjectSpawnerManager.Instance.isOutlineAllowed)
        //        //{
        //        //    outlineShader.enabled = true;
        //        //}

        //        InsertBatteryRpc(item);
        //        return;
        //    }
        //}
        base.TakeItem(owner);
    }

    private void InsertBatteryRpc(Item item, NetworkPlayerController owner)
    {
        

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

    #region insert c/s
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
    #endregion

    #endregion
}
