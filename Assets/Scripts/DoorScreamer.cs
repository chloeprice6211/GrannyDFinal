using Unity.Netcode;
using UnityEngine;

public class DoorScreamer : Mirror.NetworkBehaviour
{
    public bool isActive;
    public GameObject ghostObject;
    public AudioSource audioS;
    public AudioClip clip;
    public Animation anim;
    public Door door;
    public void OpenGhostDoor()
    {
        if(!isActive) return; 

        ghostObject.SetActive(true);
        anim.Play();
        Invoke(nameof(DeleteGhost), 1f);
        Debug.Log("DOOR SCREAMER");
        NetworkPlayerController.NetworkPlayer._cameraController.ShakeCamera(.2f, 1.5f, 1f);
        AudioSource.PlayClipAtPoint(clip, transform.position, 1.1f);
        isActive = false;
    }

    void DeleteGhost(){
        ghostObject.SetActive(false);
    }
}
