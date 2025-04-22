using Cinemachine;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class FocusObject : NetworkBehaviour, IInteractableRpc, IHighlight
{
    public CinemachineVirtualCamera focusObjectVirtualCamera;
    [SerializeField] Canvas canvas;

    [SerializeField] GameObject controlsPanel;

    [SerializeField] Outline outline;

    public Light lightSource;

    NetworkPlayerController _playerController;

    PlayerControls controls;
    InputAction releaseAction;
    Device _affectedDevice;

    protected Collider focusObjectCollider;

    [SerializeField] List<TMP_InputField> inputFields;

    [TextArea]
    public string oneTimeTip;
    public string oneTimeAudioKey;
    bool _hasTipDisplayed;
    protected bool isOperated;


    private void Awake()
    {
        focusObjectCollider = GetComponent<Collider>();
        controls = new();
        releaseAction = controls.FocusObject.Release;

        releaseAction.performed += ReleasePerformed;
    }

    private void ReleasePerformed(InputAction.CallbackContext obj)
    {
        Release();
    }

    public virtual void Interact(NetworkPlayerController owner)
    {
        foreach (TMP_InputField inputField in inputFields)
        {
            inputField.interactable = true;
        }

        if(lightSource != null){
            lightSource.enabled = true;
        }

        isOperated = true;

        //UIManager.Instance.ShowCentralTip("[ESC] LEAVE");

        controlsPanel.SetActive(true);

        owner.inventoryActionMap.Disable();
        owner.inspectInput.Disable();
        owner.pauseInput.Disable();

        UIManager.Instance.uiInventory.HideQuickSlots();

        owner.inventory.localScale = Vector3.zero;

        if (Inventory.Instance.GetMainItem(owner) != null)
        {
            Item _item = Inventory.Instance.GetMainItem(owner);

            if (_item is Device)
            {
                (_item as Device).DisableItemControls();
                _affectedDevice = _item as Device;
            }      
        }

        canvas.worldCamera = owner.playerCamera;
        

        _playerController = owner;
        owner.virtualCamera.gameObject.SetActive(false);
        focusObjectVirtualCamera.gameObject.SetActive(true);

        owner.DisablePlayerControls(true, false, true, true, true);
        releaseAction.Enable();

        UIManager.Instance.DisplayMouse();
        UIManager.Instance.HideCrosshair();

        owner._operatedFocusObject = this;
        owner.isBusy = true;

        if (oneTimeTip.Length > 0 && !_hasTipDisplayed)
        {
            UIManager.Instance.Message(oneTimeTip, oneTimeAudioKey);
            _hasTipDisplayed = true;
        }

        InteractCmd();
    }

    public virtual void Release()
    {
        //UIManager.Instance.HideCentralTip();

        controlsPanel.SetActive(false);
        isOperated = false;

        _playerController.pauseInput.Enable();
        _playerController.inventoryActionMap.Enable();

        _playerController.virtualCamera.gameObject.SetActive(true);
        focusObjectVirtualCamera.gameObject.SetActive(false);

        UIManager.Instance.uiInventory.ShowQuickSlots();

        releaseAction.Disable();
        _playerController.EnablePlayerControls();

        _playerController.inventory.localScale = Vector3.one;

        if(_affectedDevice != null)
        {
            _affectedDevice.ReturnControls();
            _affectedDevice = null;
        }

        _playerController._operatedFocusObject = null;

        UIManager.Instance.ShowCrosshair();
        UIManager.Instance.LockMouse();
        _playerController.isBusy = false;

        canvas.worldCamera = null;

        foreach (TMP_InputField inputField in inputFields)
        {
            inputField.interactable = false;
        }

        if(lightSource !=null){
            lightSource.enabled = false;
        }

        ReleaseCmd();
    }


    #region c/s
    [Command(requiresAuthority = false)]
    void InteractCmd()
    {
        InteractRpc();
    }

    [ClientRpc]
    void InteractRpc()
    {
        if(focusObjectCollider != null) focusObjectCollider.enabled = false;
    }


    [Command(requiresAuthority = false)]
    void ReleaseCmd()
    {
        ReleaseRpc();
    }

    [ClientRpc]
    void ReleaseRpc()
    {
        if(focusObjectCollider != null)focusObjectCollider.enabled = true;
    }

    #endregion

    public void Highlight()
    {
        if (outline != null)
            outline.enabled = true;
    }

    public void DisableHighlight()
    {
        if (outline != null)
            outline.enabled = false;
    }

    public void ChangeHighlightThickness(float value)
    {

    }
}
