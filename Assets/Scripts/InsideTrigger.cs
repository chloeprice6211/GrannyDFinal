using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InsideTrigger : MonoBehaviour
{
    [SerializeField] AmbienceScript ambience;

    [SerializeField] AudioSource thisSource;
    [SerializeField] AudioSource otherSource;

    public bool inside;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && other.GetComponent<NetworkPlayerController>().hasAuthority)
        {
            if(ambience.isInside != inside)
            {
                ambience.isInside = inside;
                ambience.Change(thisSource, otherSource);
            }
        }
    }
}
