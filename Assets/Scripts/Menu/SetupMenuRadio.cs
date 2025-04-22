using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SetupMenuRadio : MonoBehaviour
{
    private AudioSource _audioSource;
    [SerializeField] List<AudioClip> clips;

    [TextArea]
    [SerializeField] List<string> messages = new();
    [SerializeField] Animation messageAnimation;
    [SerializeField] TextMeshProUGUI dynamicTextField;

    private float _elapsedTime = 0;
    private float _timeToPlay;
    private float _cooldown = 20;
    float _radioMessageDisplayTime = 6;


    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _timeToPlay = Random.Range(8, 16);
    }
    private void Update()
    {
        if(_cooldown > 0)
        {
            _cooldown -= Time.deltaTime;

            return;
        }

        _radioMessageDisplayTime += Time.deltaTime;

        if (_radioMessageDisplayTime >= 9)
        {
            DisplayMessage();
            _radioMessageDisplayTime = 0;
        }

      
        _elapsedTime += Time.deltaTime;

        if (_elapsedTime >= _timeToPlay)
        {
            if (!_audioSource.isPlaying)
            {
                _audioSource.PlayOneShot(clips[Random.Range(0, clips.Count)]);
            }

            _timeToPlay = Random.Range(10, 20);
            _cooldown = 40;

            _elapsedTime = 0;
        } 
      
    }

    void DisplayMessage()
    {
        dynamicTextField.text = messages[Random.Range(0, messages.Count)];
        messageAnimation.Play();
    }   
}
