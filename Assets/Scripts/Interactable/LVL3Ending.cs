using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LVL3Ending : MonoBehaviour, ITriggered
{
    [SerializeField] AudioSource _audioSource;
    [SerializeField] GhostInteractableObject _radio;
    [SerializeField] AudioSource _doorSlamSource;

    public void OnTrigger()
    {
        NetworkPlayerController.NetworkPlayer.EnterScaredMode(TimeRange.Long, ScaredModeProperty.FearMode);
        _audioSource.Play();
        _radio.PerformGhostInteraction();
        _doorSlamSource.Play();

        Invoke("Message", 1f);
    }

    void Message()
    {
        UIManager.Instance.Message("run", "lvl3Run");
    }

}
