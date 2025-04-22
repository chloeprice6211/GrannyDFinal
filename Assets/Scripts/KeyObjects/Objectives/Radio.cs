using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public class Radio : Mirror.NetworkBehaviour
{
    [SerializeField] List<AudioClip> clips;
    [SerializeField] TextMeshProUGUI radioText;
    [SerializeField] TextMeshProUGUI message;

    [SerializeField] AudioSource audioSource;
    [SerializeField] GameObject triggerZone;


    private void Start()
    {
        GhostEvent.Instance.OnHuntStart.AddListener(HandleHuntStartCommand);
        GhostEvent.Instance.OnHuntEnd.AddListener(HandleHuntEnd);
    }

    [ServerCallback]
    private void HandleHuntStartCommand()
    {
        if (Random.Range(0, 100) > 75) return;

        int randomClipIndex = Random.Range(0, clips.Count);
        TurnRadioOnRpc(randomClipIndex);

    }

    [ClientRpc]
    private void TurnRadioOnRpc(int clipIndex)
    {
        audioSource.PlayOneShot(clips[clipIndex]);
        message.enabled = true;
    }

    private void HandleHuntEnd()
    {
        message.enabled = false;
    }

    public void PlayOnTrigger()
    {
        audioSource.PlayOneShot(clips[0]);
        Destroy(triggerZone);
    }
}
