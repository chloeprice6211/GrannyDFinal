using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fridge : LightDependentElement
{
    [SerializeField] Light lightSource;

    private float _lightIntensity;

    private void Start()
    {
        _lightIntensity = lightSource.intensity;
    }

    public void EnableLight()
    {
        lightSource.enabled = true;
    }
    
    public void DisableLight()
    {
        lightSource.enabled = false;
    }

    public override void OnLightTurnOff()
    {
        lightSource.intensity = 0;
    }

    public override void OnLightTurnOn()
    {
        lightSource.intensity = _lightIntensity;
    }
}
