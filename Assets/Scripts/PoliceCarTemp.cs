using UnityEngine;

public class PoliceCarTemp : MonoBehaviour
{
    public Animation anim;
    public AudioClip audioClip;

    public void OnAnim(){
        anim.Play();
        AudioSource.PlayClipAtPoint(audioClip, anim.transform.position);   
        }
}
