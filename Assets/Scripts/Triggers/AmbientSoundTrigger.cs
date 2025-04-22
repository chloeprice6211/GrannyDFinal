using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AmbientSoundTrigger : MonoBehaviour
{
    [SerializeField] AudioClip ambientAudio;
    public Collider _collider;

    void OnTriggerEnter(Collider other){
        if(other.gameObject.CompareTag("Player")){
            _collider.enabled = false;
            UIManager.Instance.PlaySound(ambientAudio, .55f);
        }

    }
}
