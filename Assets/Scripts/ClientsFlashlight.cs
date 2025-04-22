using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientsFlashlight : MonoBehaviour
{
    [SerializeField] Renderer _renderer;

    [SerializeField] Material activeMat;
    [SerializeField] Material inactiveMat;
    [SerializeField] Light lightSource;

    [SerializeField] AudioClip switchSound;

    bool isOn;

    public void Switch()
    {
        isOn = !isOn;

        AudioSource.PlayClipAtPoint(switchSound, transform.position);

        if (isOn)
        {
            _renderer.material = activeMat;
            lightSource.enabled = true;
        }
        else
        {
            _renderer.material = inactiveMat;
            lightSource.enabled = false;
        }
    }
}
