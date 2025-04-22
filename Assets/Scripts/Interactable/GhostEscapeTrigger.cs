using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostEscapeTrigger : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;

    public void Activate()
    {
        audioSource.Play();
        NetworkPlayerController.NetworkPlayer.EnterScaredMode(TimeRange.Long, ScaredModeProperty.FearMode);
    }
}
