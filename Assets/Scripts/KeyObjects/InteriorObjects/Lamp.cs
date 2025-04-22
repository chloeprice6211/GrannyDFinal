using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Lamp : LightDependentElement, IInteractableRpc, IGhostInteractable
{
    [SerializeField] Light lightSource;
    [SerializeField] AudioClip lampSwitch;
    [SerializeField] Animation glowing;
    [SerializeField] Renderer glowingRenderer;

    const string keyWord = "_EMISSION";

    public bool isOn;
    private AudioSource _audioSource;


    private void Awake()
    {
        glowingRenderer.material.DisableKeyword(keyWord);
        _audioSource = GetComponent<AudioSource>();
        
    }
    private void Start()
    {
        if (isOn)
        {
            SwitchOnLight();
        }
    }

    public void Interact(NetworkPlayerController owner)
    {
        if (!isPowered)
        {
            UIManager.Instance.Message("powerless", "powerlessv1_A");
        }
        SwitchLightCommand();
    }

    [Command(requiresAuthority = false)]
    private void SwitchLightCommand()
    {
        SwitchLightRcp();
    }

    [ClientRpc]
    private void SwitchLightRcp()
    {
        AudioSource.PlayClipAtPoint(lampSwitch, transform.position, .5f);

        if (!isOn)
        {
            isOn = true;
            if (!isPowered) return;
            SwitchOnLight();
        }
        else
        {
            isOn = false;
            if (!isPowered) return;
            SwitchOffLight();
        }
    }

    #region Switch

    private void SwitchOnLight()
    {
        lightSource.enabled = true;

        glowingRenderer.material.EnableKeyword(keyWord);
        tag = lightOnTag;

        if (_audioSource != null)
        {
            _audioSource.Play();
        }
    }

    private void SwitchOffLight()
    {
        glowingRenderer.material.DisableKeyword(keyWord);
        lightSource.enabled = false;
        tag = lightOffTag;

        if (_audioSource != null)
        {
            _audioSource.Stop();
        }
    }

    #endregion

    [ClientRpc]
    public void PerformGhostInteraction()
    {
        if (!isPowered) return;

        SwitchLightCommand();
    }


    public override void OnLightTurnOn()
    {
        isPowered = true;
        if (!isOn) return;

        SwitchOnLight();

    }

    public override void OnLightTurnOff()
    {
        SwitchOffLight();
        isPowered = false;
    }

}
