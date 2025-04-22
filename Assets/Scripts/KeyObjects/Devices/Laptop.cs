using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;

public class Laptop : Device, IInsertable
{
    [SerializeField] Transform flashDrivePosition;

    [Header("UI")]
    [SerializeField] GameObject welcomeScreen;
    [SerializeField] GameObject mainScreen;

    [SerializeField] Animation screenAnimation;
    [SerializeField] Animation usbInsertAnimation;
    [SerializeField] Light lightSource;

    [SerializeField] AudioClip laptopOpening;
    [SerializeField] AudioClip laptopClosing;

    [SerializeField] GameObject hiddenFolder;

    public bool isOn;
    public bool hasFlashDrive;
    public DesktopApplication currentApplication;

    //functionality
    public void TurnDisplayOn()
    {
        AudioSource.PlayClipAtPoint(laptopOpening, transform.position, .5f);
        isOn = true;
        canvas.enabled = true;
        lightSource.enabled = true;

    }

    public void TurnDisplayOff()
    {
        AudioSource.PlayClipAtPoint(laptopClosing, transform.position, .5f);
        isOn = false;
        canvas.enabled = false;
        lightSource.enabled = false;

    }

    public void ShowHiddenFolder()
    {
        hiddenFolder.SetActive(true);
    }

    #region input system

    public override void ExitDevice(InputAction.CallbackContext obj)
    {
        if (screenAnimation.isPlaying) return;

        if (!_owner.hasAuthority) return;

        CmdExitDevice();
        base.ExitDevice(obj);

    }

    public override void UseDevice(InputAction.CallbackContext obj)
    {
        if (screenAnimation.isPlaying) return;
        if (isBeingInspected) return;
        if (inspectAnimation.isPlaying) return;

        if (!_owner.hasAuthority) return;

        CmdUseDevice();
        base.UseDevice(obj);

    }

    #endregion

    #region remote override actions

    [Command(requiresAuthority = false)]
    private void CmdUseDevice()
    {
        RpcUseDevice();
    }
    [ClientRpc]
    private void RpcUseDevice()
    {
        TurnDisplayOn();
        screenAnimation.Play("LaptopScreenUp");
    }

    [Command(requiresAuthority = false)]
    private void CmdExitDevice()
    {
        RpcExitDevice();
    }
    [ClientRpc]
    private void RpcExitDevice()
    {
        Invoke("TurnDisplayOff", .6f);
        screenAnimation.Play("LaptopScreenDown");
    }

    #endregion

    //button listeners
    public void OnEnterClick()
    {
        CmdEnterSystem();
    }

    #region enter system remote action

    //UI management
    public void EnterSystem()
    {
        welcomeScreen.SetActive(false);
        mainScreen.SetActive(true);
    }

    [Command(requiresAuthority = false)]
    private void CmdEnterSystem()
    {
        RpcEnterSystem();
    }
    [ClientRpc]
    private void RpcEnterSystem()
    {
        EnterSystem();
    }

    #endregion

    #region insert flash c/s

    public void Insert(UtilityItem itemToInsert, NetworkPlayerController owner)
    {
        Debug.Log("inserted");
        itemToInsert.OnItemUsed(owner);

        //if (ObjectSpawnerManager.Instance.isOutlineAllowed)
        //{
        //    outlineShader.enabled = true;
        //}

        UIManager.Instance.Message("flashWonder", "flashWonder_A");

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
        item.OnInsert();
        hasFlashDrive = true;

        Transform parentItem = item.transform.parent;

        parentItem.SetParent(flashDrivePosition);
        parentItem.localPosition = Vector3.zero;
        parentItem.localRotation = Quaternion.Euler(Vector3.zero);
        item.GetComponent<Collider>().enabled = false;

        parentItem.localScale = item.parentScale;

        usbInsertAnimation.Play();
        ShowHiddenFolder();

    }

    #endregion
}
