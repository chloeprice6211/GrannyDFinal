using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingAudio : MonoBehaviour
{
    public AudioClip endingAudioClip;
    public AudioSource audioSource;

    void Start(){
        DontDestroyOnLoad(gameObject);
    }

    public void PlayEndingAudio(){
        audioSource.Play();
    }
}
