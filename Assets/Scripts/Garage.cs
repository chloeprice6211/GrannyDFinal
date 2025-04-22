using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Garage : MonoBehaviour, IUnlockable
{
    [SerializeField] Animation openAnimation;
    [SerializeField] AudioSource aSource;

    public void OpenGate()
    {
        openAnimation.Play();
        aSource.Play();
    }
    

    public void Unseal()
    {
        OpenGate();
    }

    public bool Check()
    {
        throw new System.NotImplementedException();
    }
}
