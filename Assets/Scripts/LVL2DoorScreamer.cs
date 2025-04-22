using Unity.VisualScripting;
using UnityEngine;

public class LVL2DoorScreamer : MonoBehaviour
{
    public AudioClip audioClip;
    public Collider _collider;
    void OnTriggerEnter(Collider other){
        if(other.tag == "Player"){
             _collider.enabled = false;
            Invoke(nameof(InvokeScreamer), 2f);
            
        }
       
    }

    void InvokeScreamer(){
        AudioSource.PlayClipAtPoint(audioClip, transform.position);
    }
}
