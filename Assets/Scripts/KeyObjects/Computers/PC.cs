using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PC : LightDependentElement, IInteractableRpc, IGhostInteractable, IUSBInsertable
{
    [SerializeField] NewMonitor monitor;
    [SerializeField] AudioClip switchSound;
    [SerializeField] AudioClip pcStartupSound; 
    [SerializeField] Material glowingMaterial;
    [SerializeField] Animation turnPcAnimation;
    [SerializeField] Light lightSource;
    [SerializeField] GameObject hiddenFolder;

    private AudioSource _audioSource;
    private bool _isOn;

    public bool isBroken;
    public bool usb;

    public Transform usbInsertPosition;


    private void Awake()
    {
    }
    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void Interact(NetworkPlayerController owner)
    {
        if (usb)
        {
            if (Inventory.Instance.HasAnyItem(owner))
            {
                if (Inventory.Instance.GetMainItem(owner) is FlashDriver)
                {
                    FlashDriver usb = Inventory.Instance.GetMainItem(owner) as FlashDriver;
                    owner.InventoryScript.ClearItem(owner.InventoryScript.items.IndexOf(usb));
                    InsertUSB(usb);
                    return;
                }
            }
        }

        AudioSource.PlayClipAtPoint(switchSound, transform.position, .5f);

        if (isBroken)
        {
            UIManager.Instance.Message("noBootPC", "");
            return;
        }
        if (!isPowered) 
        {
            UIManager.Instance.Message("powerless", "powerlessv1_A");
            return;
        }
        
        if (!_isOn)
            TurnOnCommand();
        else
            TurnOffCommand();
    }

    #region Turn on c/s
    [Command (requiresAuthority = false)]
    public void TurnOnCommand()
    {
        TurnOnRpc();
    }
    [ClientRpc]
    private void TurnOnRpc()
    {
        TurnOnPC();
    }

    private void TurnOnPC()
    {
        _isOn = true;
        _audioSource.Play();
        AudioSource.PlayClipAtPoint(pcStartupSound, transform.position);
        tag = lightOnTag;
        lightSource.enabled = true;

        Invoke("TurnMonitorOn", 3f);

    }

    [Command (requiresAuthority = false)]
    public void TurnOffCommand()
    {
        TurnOffRpc();
    }
    [ClientRpc]
    private void TurnOffRpc()
    {
        TurnOffPc();
    }

    private void TurnOffPc()
    {
        _audioSource.Stop();
        _isOn = false;
        tag = lightOffTag;

        lightSource.enabled = false;
        monitor.TurnMonitorOff();
        CancelInvoke();

    }

    private void TurnMonitorOn()
    {
        if (!_isOn) return;

        monitor.TurnMonitorOn();
    }

    #endregion


    //event handlers
    public void PerformGhostInteraction()
    {
        if (!isPowered) return;

        if (_isOn)
        {
            TurnOffPc();
        }
        else
        {
            TurnOnPC();
        }
    }

    public override void OnLightTurnOn()
    {
        isPowered = true;

        if (_isOn)
        {
            TurnOnPC();
        }
    }

    public override void OnLightTurnOff()
    {
        isPowered = false;

        if (_isOn)
        {
            TurnOffPc();
        }
    }

    public void InsertUSB(FlashDriver usb)
    {
        InsertUSBCmd(usb);   
    }

    [Command(requiresAuthority = false)]
    private void InsertUSBCmd(FlashDriver flash)
    {
        InsertUSBRpc(flash);
    }
    [ClientRpc]
    private void InsertUSBRpc(FlashDriver flash)
    {
        flash.TurnFlashOn();

        Transform parentItem = flash.transform.parent;

        parentItem.SetParent(usbInsertPosition);
        parentItem.localPosition = Vector3.zero;
        parentItem.localRotation = Quaternion.Euler(Vector3.zero);

        usbInsertPosition.GetComponent<Animation>().Play();

        ShowHiddenFile();
    }

    public void ShowHiddenFile()
    {
        hiddenFolder.SetActive(true);
    }
}
