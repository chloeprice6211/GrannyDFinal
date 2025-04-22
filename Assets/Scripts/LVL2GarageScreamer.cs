using UnityEngine;

public class LVL2GarageScreamer : MonoBehaviour
{
    public AudioSource audioSource;
    AudioClip clip;

    void OnTriggerEnter(Collider other){
        if(other.tag == "Player"){
            InvokeTrigger();
            GetComponent<Collider>().enabled = false;
        }
    }

    void InvokeTrigger(){
        audioSource.Play();
    }
}
