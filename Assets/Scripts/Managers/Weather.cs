using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weather : MonoBehaviour
{

    [SerializeField] List<AudioClip> thunderClips;
    [SerializeField] AudioSource audioSource;

    float _timeToThunder;
    float _elapsedTime;

    public float timeToThunderFrom;
    public float timeToThunderTo;

    private void Start()
    {
        _timeToThunder = Random.Range(timeToThunderFrom, timeToThunderTo);
    }

    void Update()
    {
        _elapsedTime += Time.deltaTime;
        
        if(_elapsedTime >= _timeToThunder)
        {
            audioSource.PlayOneShot(thunderClips[Random.Range(0, thunderClips.Count)]);
            _timeToThunder = Random.Range(timeToThunderFrom, timeToThunderTo);

            _elapsedTime = 0;
        }
    }
}
