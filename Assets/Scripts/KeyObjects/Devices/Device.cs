using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public abstract class Device : Item, IHasControls
{
    //components
    protected NetworkPlayerController _playerController;
    protected PlayerCamera _playerCamera;
    protected PlayerControls _controls;

    public PlayerControls Controls
    {
        get
        {
            return _controls;
        }
    }

    [SerializeField] List<GameObject> controlCanvasElements;

    //input actions
    public InputAction UseDeviceAction => _controls.Device.Use;
    public InputAction ExitDeviceAction => _controls.Device.Exit;

    [SerializeField] protected AnimationClip exitDeviceClip;
    [SerializeField] protected AnimationClip useDeviceClip;

    public Animation deviceInteractAnimation;
    [SerializeField] protected Canvas canvas;
    [SerializeField] protected Canvas controlsCanvas;

    protected TMP_InputField _selectedInputField = null;

    public bool isBeingUsed;
    private bool _hasTipDisplayed;

    public UnityEvent OnDeviceOff;
   


    public override void Awake()
    {
        base.Awake();
        _controls = new();

        UseDeviceAction.started += UseDevice;
        ExitDeviceAction.started += ExitDevice;

        deviceInteractAnimation.AddClip(useDeviceClip, useDeviceClip.name);
        deviceInteractAnimation.AddClip(exitDeviceClip, exitDeviceClip.name);

       

    }

    void Start(){
         //OnDeviceOff.AddListener( NetworkPlayerController.NetworkPlayer.Scream);
    }


    //input controls
    public virtual void UseDevice(InputAction.CallbackContext context)
    {
        if (!_owner.hasAuthority) return;

        if (_owner.isDropCommandBeingPerformed) return;

        if (isBeingInspected) return;
        if (inspectAnimation.isPlaying) return;

        if (deviceInteractAnimation.isPlaying) return;

        controlCanvasElements[0].SetActive(false);
        controlCanvasElements[1].SetActive(true);

        if (UIManager.Instance.uiInventory.IsActive)
            UIManager.Instance.uiInventory.Close(false);

        UIManager.Instance.HideCrosshair();
        UIManager.Instance.HideInteractOption();

        UIManager.Instance.uiInventory.HideQuickSlots();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.lockState = CursorLockMode.None;

        _owner.pauseInput.Disable();
        _owner.isBusy = true;

        if (_owner.flashlight != null && _owner.flashlight.isOn)
            _owner.flashlight.SwitchFlashlight(false);

        Cursor.visible = true;

        _owner.DisablePlayerControls(true, true, true, true);

        if(_owner.flashlight != null){
             _owner.flashlight.DisableControls();
        }
       
        _owner.inventoryActionMap.Disable();
        _playerCamera.CenterCamera();

        UseDeviceAction.Disable();
        ExitDeviceAction.Enable();

        isBeingUsed = true;

        CmdUseDevice();
    }

    public virtual void ExitDevice(InputAction.CallbackContext context)
    {
        if (!_owner.hasAuthority) return;
        if (_owner.isDropCommandBeingPerformed) return;

        if (deviceInteractAnimation.isPlaying) return;

        _owner.pauseInput.Enable();
        _owner.inventoryActionMap.Enable();

        UIManager.Instance.uiInventory.ShowQuickSlots();

        UIManager.Instance.ShowCrosshair();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        controlCanvasElements[0].SetActive(true);
        controlCanvasElements[1].SetActive(false);

        isBeingUsed = false;

        if (_owner.flashlight != null && !_owner.flashlight.isOn)
            _owner.flashlight.SwitchFlashlight(false);

        transform.parent.SetParent(_playerController.inventory, true);

        _owner.EnablePlayerControls();

        UseDeviceAction.Enable();
        ExitDeviceAction.Disable();

        if(_owner.flashlight != null){
 _owner.flashlight.EnableControls();
        }
       

        if(_selectedInputField != null)
        {
            _selectedInputField.DeactivateInputField();
        }

        _owner.isBusy = false;
        OnDeviceOff?.Invoke();

        ExitDeviceCommand();

    }

    #region remote actions

    [Command (requiresAuthority = false)]
    private void CmdUseDevice()
    {
        RpcUseDevice();
    }
    [ClientRpc]
    private void RpcUseDevice()
    {
        deviceInteractAnimation.Play(useDeviceClip.name);
    }

    [Command(requiresAuthority = false)]
    private void ExitDeviceCommand()
    {
        ExitDeviceRpc();
    }
    [ClientRpc]
    private void ExitDeviceRpc()
    {
        deviceInteractAnimation.Play(exitDeviceClip.name);
    }

    #endregion

    public void OnInputFieldSelect(TMP_InputField inputField)
    {
        _selectedInputField = inputField;
        Debug.Log("selected input field");
    }



    //interface implementation
    public void DisableItemControls()
    {
        UseDeviceAction.Disable();
        controlsCanvas.enabled = false;
        //UIManager.Instance.HideNoRayInteraction();
    }

    public override void OnDropItem()
    {
        if (deviceInteractAnimation.isPlaying)
        {
            deviceInteractAnimation.Stop();
        }

        controlsCanvas.enabled = false;
        base.OnDropItem();
        UseDeviceAction.Disable();
        ExitDeviceAction.Disable();
    }

    public override void Inspect()
    {
        if (deviceInteractAnimation.isPlaying) return;
        if (isBeingUsed) return;

        controlsCanvas.enabled = false;
        base.Inspect();
    }

    public override void ReturnFromInspect()
    {
        //controlsCanvas.enabled = true;
        //base.ReturnFromInspect();
    }

    public void ReturnControls()
    {
        controlsCanvas.enabled = true;
        UseDeviceAction.Enable();
        //UIManager.Instance.ShowNoRayInteraction("USE", Keys.F);
    }

    public override void TakeItem(NetworkPlayerController item)
    {
        base.TakeItem(item);

        if (!_owner.hasAuthority) return;

        controlsCanvas.enabled = true;

        _playerController = item.GetComponent<NetworkPlayerController>();
        _playerCamera = item.GetComponent<PlayerCamera>();

        canvas.worldCamera = _playerController.playerCamera;

        UseDeviceAction.Enable();

    }

    void OnDisable()
    {
        _controls.Disable();
    }

}
