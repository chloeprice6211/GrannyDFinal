using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorKnockoutTrigger : MonoBehaviour
{
    [SerializeField] Animation doorAnimation;
    [SerializeField] AudioSource doorSource;
    [SerializeField] AudioSource ghostSource;

    [SerializeField] AudioClip doorKnock;
    [SerializeField] AudioClip ghostClip;

    bool _canActivate = true;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && _canActivate)
        {
            NetworkPlayerController _player = other.GetComponent<NetworkPlayerController>();
            if (!Inventory.Instance.HasAnyItem(_player)) return;

            Item _item = Inventory.Instance.GetMainItem(_player);

            if (_item is Key && (_item as Key).objectiveType == ObjectiveType.LVL2_Gate)
            {
                _canActivate = false;
                Activate();
            }
        }
    }

    public void Activate()
    {
        doorAnimation.Play();
        doorSource.PlayOneShot(doorKnock);
        ghostSource.PlayOneShot(ghostClip);
        NetworkPlayerController.NetworkPlayer.EnterScaredMode(TimeRange.Long, ScaredModeProperty.FearMode);

        Invoke(nameof(DelayedMessage), 1f);
    }

    void DelayedMessage()
    {
        UIManager.Instance.Message("god", "ohGodV1_A");
    }
}
