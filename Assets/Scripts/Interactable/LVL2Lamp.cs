using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LVL2Lamp : LightDependentElement
{
    [SerializeField] Material activeMat;
    [SerializeField] Material inactiveMat;
    [SerializeField] Renderer _renderer;
    [SerializeField] Light lightSource;

    [SerializeField] Animation _animation;

    public bool isOn;

    float _intensity;

    private void Start()
    {
        _intensity = lightSource.intensity;

        GhostEvent.Instance.OnHuntStart.AddListener(OnHuntStart);
        GhostEvent.Instance.OnHuntEnd.AddListener(OnHuntEnd);
    }

    public override void OnLightTurnOff()
    {
        TurnOff();
        isPowered = false;
    }

    public override void OnLightTurnOn()
    {
        isPowered = true;
        if (isOn)
        {
            TurnOn();
        }
       
    }

    public void Switch()
    {
        if (!isOn)
        {
            if (isPowered)
            {
                TurnOn();
            }
        }
        else
        {
            TurnOff();
        }

        isOn = !isOn;
    }

    public void TurnOn()
    {
        _renderer.material = activeMat;
        lightSource.enabled = true;
    }

    public void TurnOff()
    {
        _renderer.material = inactiveMat;
        lightSource.enabled = false;
    }

    void OnHuntStart()
    {
        if (_animation != null)
            _animation.Play();
    }

    void OnHuntEnd()
    {
        if (_animation != null)
        {
            float _currentIntensity = lightSource.intensity;
            _animation.Stop();
            lightSource.intensity = _currentIntensity;

            StartCoroutine(ReturnIntensityRoutine());
        }


    }

    IEnumerator ReturnIntensityRoutine()
    {
        if(lightSource.intensity < _intensity)
        {
            while(lightSource.intensity <= _intensity)
            {
                lightSource.intensity += .35f * Time.deltaTime;
                yield return null;
            }

            lightSource.intensity = _intensity;
        }
        else if (lightSource.intensity < _intensity)
        {
            while (lightSource.intensity >= _intensity)
            {
                lightSource.intensity -= .35f * Time.deltaTime;
                yield return null;
            }

            lightSource.intensity = _intensity;
        }

    }
}
