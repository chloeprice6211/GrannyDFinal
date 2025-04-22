using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostInteractableObject : NetworkBehaviour, IGhostInteractable, IInteractableRpc
{
    [SerializeField] List<AudioClip> clips;
    [SerializeField] AudioSource _audioSource;

    [SerializeField] AudioClip stopClip;

    int count = 0;

    public int chanceToTriggerOnHunt;

    private void Start()
    {
        GhostEvent.Instance.OnHuntStart.AddListener(OnHuntStart);
    }

    public void Interact(NetworkPlayerController owner)
    {
        if (_audioSource.isPlaying)
            StopCmd();
    }

    [ClientRpc]
    void RpcPerform()
    {
        PerformGhostInteraction();
    }

    public void PerformGhostInteraction()
    {
        if (count >= clips.Count)
            count = 0;

        _audioSource.clip = clips[count];
        _audioSource.Play();
        gameObject.layer = 7;

        Invoke("Stop", _audioSource.clip.length);

        count++;
    }

    [Command (requiresAuthority = false)]
    void StopCmd()
    {
        StopRpc();
    }
    [ClientRpc]
    void StopRpc()
    {
        Stop();
    }

    void Stop()
    {
        CancelInvoke("Stop");

        _audioSource.Stop();
        _audioSource.PlayOneShot(stopClip);
        gameObject.layer = 0;
    }

    [ServerCallback]
    void OnHuntStart()
    {
        if (Random.Range(0, 100) <= chanceToTriggerOnHunt && !_audioSource.isPlaying)
            RpcPerform();
    }

}
