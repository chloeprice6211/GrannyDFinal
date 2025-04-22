using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Shelf : Mirror.NetworkBehaviour, IInteractableRpc
{
    bool isOpen;
    Animation anim;
    [SerializeField] string closingAnimationName;

    [SerializeField] AudioClip opening;
    [SerializeField] AudioClip closing;

    private void Start()
    {
        anim = GetComponent<Animation>();
        
    }
    public void Interact(NetworkPlayerController owner)
    {
        if (!isOpen)
        {
            anim.Play();
            isOpen = true;

            AudioSource.PlayClipAtPoint(opening, transform.position);
        }
        else
        {
            anim.PlayQueued(closingAnimationName);
            isOpen = false;

            AudioSource.PlayClipAtPoint(closing, transform.position);
        }
            
    }
}
