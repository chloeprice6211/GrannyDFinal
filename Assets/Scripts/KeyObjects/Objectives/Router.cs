using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Mirror;
using TMPro;

public class Router : LightDependentElement, IGenerable
{
    public bool isOn;

    public UnityEvent OnRouterOn;
    public UnityEvent OnRouterOff;

    [SerializeField] Material lightIndicatorMaterial;
    [SerializeField] List<Light> lightSources;
    [SerializeField] TextMeshProUGUI codeTextField;

    [SyncVar] public string passcode;
    private const string keyWord = "_EMISSION";


    private void Awake()
    {
        lightIndicatorMaterial.DisableKeyword(keyWord);
    }

    public override void OnLightTurnOn()
    {    
        OnRouterOn?.Invoke();
        TurnRouterOn();
    }

    public override void OnLightTurnOff()
    {
        OnRouterOff?.Invoke();
        TurnRouterOff();
    }

    public void TurnRouterOn()
    {
        lightIndicatorMaterial.EnableKeyword(keyWord);

        foreach(Light light in lightSources)
        {
            light.enabled = true;
        }
    }

    public void TurnRouterOff()
    {
        lightIndicatorMaterial.DisableKeyword(keyWord);

        foreach (Light light in lightSources)
        {
            light.enabled = false;
        }
    }

    public void ApplyGeneratedCode(string code)
    {
        passcode = code;
    }

    public virtual void ShowGeneratedCode()
    {
        codeTextField.text = "wifi: " + passcode;
    }
}
