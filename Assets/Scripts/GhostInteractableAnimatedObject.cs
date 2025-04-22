using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostInteractableAnimatedObject : MonoBehaviour, IGhostInteractable, ITriggered
{
    [SerializeField] Animation _animation;
    [SerializeField] AudioSource _audioSource;

    public void OnTrigger()
    {
        PerformGhostInteraction();
    }

    public void PerformGhostInteraction()
    {
        if (!_animation.isPlaying)
        {
            _animation.Play();
            _audioSource.Play();
        }
    }
}
