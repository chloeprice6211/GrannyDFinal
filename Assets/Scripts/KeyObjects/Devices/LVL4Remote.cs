using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;

public class LVL4Remote : Device, IInsertable
{
    [SerializeField] Material inactiveMaterial;
    [SerializeField] Material unlockedMaterial;
    [SerializeField] Material lockedMaterial;
    Material currentMaterial;
    [SerializeField] AudioClip switchSound;
    [SerializeField] AudioClip batteryInstertSound;
    [SerializeField] Renderer objectRenderer;
    [SerializeField] Light lightSource;
    public GameObject unlockableG;
    IUnlockable _unlockable;

    public bool isOn;
    public bool isPowered;
    [SyncVar] public bool canOpen = true;

    void Start()
    {
        currentMaterial = lockedMaterial;
        unlockableG.TryGetComponent<IUnlockable>(out _unlockable);
    }

    public void TurnRemoteOn()
    {
        isOn = true;
        lightSource.enabled = true;

        AudioSource.PlayClipAtPoint(switchSound, transform.position);
        objectRenderer.material = currentMaterial;

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
        canOpen = false;
        OpenGateRpc();
    }
    [ClientRpc]
    private void OpenGateRpc()
    {
        TurnRemoteOn();
        currentMaterial = unlockedMaterial;
        objectRenderer.material = currentMaterial;
        _unlockable.Unseal();
        canOpen = false;
    }

    public void OpenGate()
    {
        if(!canOpen) return;
        AudioSource.PlayClipAtPoint(switchSound, transform.position);

        if (!isPowered)
        {
            UIManager.Instance.Message("remoteBattery", "remoteBattery_A");
            return;
        }
        if(!_unlockable.Check()){
              UIManager.Instance.Message("powerless", "powerless_A");
            return;
        }

        OpenGateCommand();

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
        TurnRemoteOn();
    }

    #region On button clicks

    public void OnEnableClick()
    {
        EnableRemote();
    }

    public void OnOpenGateClick()
    {
        OpenGate();
    }

    #endregion

}
