using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VentFan : LightDependentElement
{
    private AudioSource _audioSource;
    private Animation _animation;

    [SerializeField] Collider fanCollider;


    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _animation = GetComponent<Animation>();
    }

    public void TurnOnFan()
    {
        fanCollider.enabled = true;
        _audioSource.Play();
        _animation.Play();

    }

    public void TurnOffFan()
    {
        fanCollider.enabled = false;
        _audioSource.Stop();
        _animation.Stop();

    }

    public override void OnLightTurnOn()
    {
        TurnOnFan();
    }

    public override void OnLightTurnOff()
    {
        TurnOffFan();
    }
}
