using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class DrawerCabinet : Mirror.NetworkBehaviour, IInteractableRpc
{
    [SerializeField] AudioClip drawerOpen;
    [SerializeField] AudioClip drawerClose;

    private Animation _animation;
    private bool _isOpen;

    public string[] clipNames;


    private void Start()
    {
        _animation = GetComponent<Animation>();
    }


    public void Interact(NetworkPlayerController owner)
    {
        if (_animation.isPlaying) return;
        PullOrPushCommand(); 
    }


    [Command (requiresAuthority = false)]
    public void PullOrPushCommand()
    {
        PullOrPushRpc();
    }
    [ClientRpc]
    private void PullOrPushRpc()
    {
        if (_isOpen)
        {
            _animation.Play(clipNames[1]);
            _isOpen = false;
            AudioSource.PlayClipAtPoint(drawerOpen, transform.position);
            tag = "ClosedDrawer";
        }
        else
        {
            _animation.Play(clipNames[0]);
            _isOpen = true;
            AudioSource.PlayClipAtPoint(drawerClose, transform.position);
            tag = "OpenDrawer";
        }
    }

}
