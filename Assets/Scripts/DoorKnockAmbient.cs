using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorKnockAmbient : MonoBehaviour
{
    public List<AudioClip> clips;
    public AudioSource audioSource;

    float _timer;
    float currentGoal = 350;
    int currentIndex = 0;
    void Update()
    {
        _timer += Time.deltaTime;

        if(_timer >= currentGoal){
            currentGoal += 150;
            audioSource.PlayOneShot(clips[currentIndex]);
            currentIndex++;
            _timer = 0f;

            if(currentIndex==3){
                gameObject.SetActive(false);
            }
        }
    }
}
