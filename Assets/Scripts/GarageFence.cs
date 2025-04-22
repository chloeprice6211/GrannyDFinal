using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarageFence : MonoBehaviour
{
    public bool isFanceOngoing;
    [SerializeField] GarageDoor door;
    [SerializeField] GarageIndicator indicator;
    [SerializeField] Animation fenceAnimation;
    [SerializeField] AudioSource fenceAudioSource;
    [SerializeField] AudioClip unlockingAudio;

    [SerializeField] AnimationClip unlockingClip;
    [SerializeField] AnimationClip lockingClip;

    public bool IsFenceOperating
    {
        get
        {
            return fenceAnimation.isPlaying;
        }
    }

    public void Unlock()
    {
        fenceAnimation.Play(unlockingClip.name);
        fenceAudioSource.PlayOneShot(unlockingAudio);

        StartCoroutine(AudioRoutine());
    }

    public void Lock()
    {
        fenceAnimation.Play(lockingClip.name);
        fenceAudioSource.PlayOneShot(unlockingAudio);

        StartCoroutine(AudioRoutine());
    }

    IEnumerator AudioRoutine()
    {
        while (IsFenceOperating)
        {
            if (!fenceAnimation.isPlaying)
            {
                fenceAudioSource.Play();
            }

            yield return null;
        }
    }


}
