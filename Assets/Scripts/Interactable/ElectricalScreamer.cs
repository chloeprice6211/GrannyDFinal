using Unity.VisualScripting;
using UnityEngine;

public class ElectricalScreamer : MonoBehaviour
{
    public AudioClip audioClip;
    public ParticleSystem particle;
    void OnTriggerEnter(Collider collider){
        if(collider.gameObject.CompareTag("Player")){
            InvokeScreamer();
            gameObject.GetComponent<Collider> ().enabled = false;
        }
    }
    void InvokeScreamer(){
        particle.Play();
        AudioSource.PlayClipAtPoint(audioClip, this.gameObject.transform.position ,1.25f);
    }
}
