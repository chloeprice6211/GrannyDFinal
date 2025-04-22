using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostAmbientTutorial : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] List<AudioClip> clips;

    float _timeElapsed;

    private void Update()
    {
        _timeElapsed += Time.deltaTime;

        if(_timeElapsed >= 45)
        {
            _timeElapsed = 0;
            audioSource.PlayOneShot(clips[Random.Range(0, clips.Count)]);
        }
    }
}
