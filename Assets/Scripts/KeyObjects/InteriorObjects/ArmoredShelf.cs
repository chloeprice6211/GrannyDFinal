using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmoredShelf : MonoBehaviour
{
    [SerializeField] Animation[] opening = new Animation[2];
    [SerializeField] AudioClip openingSound;

    public void OpenShelf()
    {
        AudioSource.PlayClipAtPoint(openingSound, transform.position);
        opening[0].Play();
        opening[1].Play();

    }
}
