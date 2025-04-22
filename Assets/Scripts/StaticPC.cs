using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticPC : NewMonitor
{
    [SerializeField] Renderer _renderer;

    [SerializeField] Material _turnedOnMat;
    [SerializeField] Material _turnedOffMat;

    [SerializeField] Light _lightsource;

    public override void TurnMonitorOn()
    {
        //Invoke("DelayedStart", 3f);
        _renderer.material = _turnedOnMat;
        _lightsource.enabled = true;
    }

    public override void TurnMonitorOff()
    {
        //CancelInvoke("DelayedStart");
        _renderer.material = _turnedOffMat;
        _lightsource.enabled = false;
    }

    void DelayedStart()
    {
        _renderer.material = _turnedOnMat;
        _lightsource.enabled = true;
    }
}
