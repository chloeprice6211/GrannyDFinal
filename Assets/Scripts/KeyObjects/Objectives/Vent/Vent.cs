using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vent : MonoBehaviour, IUnscrewable
{
    public int screws = 4;
    [SerializeField] GameObject ventPanel;

    private Rigidbody _ventRigidbody;

    private void Start()
    {
        _ventRigidbody = ventPanel.GetComponent<Rigidbody>();
    }

    public void RemoveScrew()
    {
        screws--;

        if(screws == 0)
        {
            UIManager.Instance.Message("fan", "fan_A");
            Release();
        }
    }

    public void Unscrew()
    {
        RemoveScrew();
    }

    public void Release()
    {
        _ventRigidbody.isKinematic = false;
    }
}
