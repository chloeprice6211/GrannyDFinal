using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;
using UnityEngine.Localization.Settings;

public class Flashlight : NetworkBehaviour, IInteractableRpc
{
    [SerializeField] AudioClip switchSound;
    [SerializeField] GameObject _light;
    [SerializeField] AudioClip grabClip;
    [SerializeField] Outline outline;

    public Light _lightSource;
    private Animation _flashlightAnimation;
    PlayerControls _controls;
    InputAction _flashlightSwitchAction;

    ClientsFlashlight _clientsFlashlight;
    NetworkPlayerController _owner;

    public bool isOn = false;

    string oneTimeTip = "flashlightWorking";
    bool hasOneTimeTipDisplayed;

    float _intensity;

    #region unity callback

    private void Awake()
    {
        _controls = new();
        _flashlightSwitchAction = _controls.Flashlight.Switch;
        _flashlightSwitchAction.performed += FlashlightSwitchPerformed;

        _intensity = _lightSource.intensity;

    }

    private void Start()
    {
        _lightSource = _light.GetComponent<Light>();
        _flashlightAnimation = _lightSource.GetComponent<Animation>();

        GhostEvent.Instance.OnHuntStart.AddListener(HandleGhostEventStart);
        GhostEvent.Instance.OnHuntEnd.AddListener(HandleGhostEventEnd);


    }

    #endregion

    #region functionality

    public virtual void SwitchFlashlight(bool playSound = true)
    {
        isOn = !isOn;
        CmdSwitch(_owner);

        if(playSound)
            AudioSource.PlayClipAtPoint(switchSound, transform.position, .3f);

        _lightSource.enabled = !_lightSource.enabled;
    }

    public virtual void Interact(NetworkPlayerController owner)
    {
        if (owner.flashlightPosition.childCount != 0)
        {
            UIManager.Instance.Message("flashlight-taken", "flashlightTaken_A");
            return;
        }

        _owner = owner;
        owner.flashlight = this;
        outline.enabled = false;
        _clientsFlashlight = owner.clientFlashlight;
        transform.SetParent(owner.flashlightPosition);

        UIManager.Instance.controlsMessageTip.ControlsMessage("flashlightTip");
        UIManager.Instance.controlsMessageTip.ControlsMessage("sprintTip");

        transform.localRotation = Quaternion.Euler(Vector3.zero);
        transform.localPosition = Vector3.zero;

        AudioSource.PlayClipAtPoint(grabClip, transform.position, .5f);

        _flashlightSwitchAction.Enable();

        CmdEnableSharedFlashlight(owner);
    }

    [Command(requiresAuthority = false)]
    void CmdEnableSharedFlashlight(NetworkPlayerController player)
    {
        RpcEnableSharedFlashlight(player);
    }
    [ClientRpc (includeOwner = false)]
    void RpcEnableSharedFlashlight(NetworkPlayerController owner)
    {
        if (owner.hasAuthority) return;

        gameObject.SetActive(false);
        owner.EnableFlashlightRig();
    }

    [Command (requiresAuthority = false)]
    void CmdSwitch(NetworkPlayerController owner)
    {
        RpcSwitch(owner);
    }
    [ClientRpc]
    void RpcSwitch(NetworkPlayerController owner)
    {
        if (owner.hasAuthority) return;

        owner.clientFlashlight.Switch();
    }
        #endregion

    #region events

    public void HandleGhostEventStart()
    {
        if (_owner == null) return;
        _flashlightAnimation.Play();
    }

    public void HandleGhostEventEnd()
    {
        if (_owner == null) return;
        _flashlightAnimation.Stop();
        StartCoroutine(ReturnIntensityRoutine());
    }

    #endregion

    #region controls

    private void FlashlightSwitchPerformed(InputAction.CallbackContext obj)
    {
        SwitchFlashlight();

        if (!hasOneTimeTipDisplayed)
        {
            UIManager.Instance.Message(oneTimeTip, "flashlightWorks_A");
            hasOneTimeTipDisplayed = true;
        }
    }

    public void DisableControls()
    {
        _flashlightSwitchAction.Disable();
    }

    public void EnableControls()
    {
        _flashlightSwitchAction.Enable();
    }

    private void OnDestroy()
    {
        _controls.Disable();
    }

    #endregion

    IEnumerator ReturnIntensityRoutine()
    {
        if (_lightSource.intensity < _intensity)
        {
            while (_lightSource.intensity <= _intensity)
            {
                _lightSource.intensity += .35f * Time.deltaTime;
                yield return null;
            }

            _lightSource.intensity = _intensity;
        }
        else if (_lightSource.intensity < _intensity)
        {
            while (_lightSource.intensity >= _intensity)
            {
                _lightSource.intensity -= .35f * Time.deltaTime;
                yield return null;
            }

            _lightSource.intensity = _intensity;
        }

    }

}
