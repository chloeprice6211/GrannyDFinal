using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CRTMonitor : LightDependentElement, IInteractableRpc
{
    [SerializeField] GameObject canvas;
    [SerializeField] Renderer _renderer;
    [SerializeField] AudioClip switchSound;
    [SerializeField] Light lightSource;
    [SerializeField] Light indicatorLightSource;

    [SyncVar] public bool isOn = false;

    string keyWord = "_EMISSION";


    //interface implementation
    public void Interact(NetworkPlayerController owner)
    {
       
        CmdTurnOnOffScreen();

        if (!isPowered)
        {
            UIManager.Instance.Message("powerless", "powerlessv1_A");
        }
    }

    [Command(requiresAuthority = false)]
    void CmdTurnOnOffScreen()
    {
        RpcTurnOnOffScreen();
    }
    [ClientRpc]
    void RpcTurnOnOffScreen()
    {
        AudioSource.PlayClipAtPoint(switchSound, transform.position, .5f);

        if (!isPowered) return;

        if (!isOn)
        {
            SwitchOn();
        }
        else
        {
            SwitchOff();
        }
    }

    void SwitchOn()
    {
        isOn = true;
        Invoke("TurnOnDelayed", 3f);
        _renderer.material.EnableKeyword(keyWord);
        indicatorLightSource.enabled = true;
        tag = lightOnTag;
    }
    void SwitchOff()
    {
        isOn = false;
        CancelInvoke("TurnOnDelayed");
        _renderer.material.DisableKeyword(keyWord);
        canvas.gameObject.SetActive(false);
        lightSource.enabled = false;
        indicatorLightSource.enabled = false;
        tag = lightOffTag;
    }

    void TurnOnDelayed()
    {
        canvas.gameObject.SetActive(true);
        lightSource.enabled = true;
    }


    private void Start()
    {
        _renderer.material.DisableKeyword(keyWord);
    }

    public override void OnLightTurnOff()
    {
        isPowered = false;

        if (isOn)
        {
            SwitchOff();
        }
    }

    public override void OnLightTurnOn()
    {
        isPowered = true;
    }
}
