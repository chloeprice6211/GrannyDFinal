using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class GhostActivationTrigger : MonoBehaviour
{
    [SerializeField] GameObject ghostManagerObject;
    [SerializeField] Collider _collider;
    
    [SerializeField] AudioClip onTriggerAudioClip;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {

            UIManager.Instance.PlaySound(onTriggerAudioClip, .225f);
            _collider.enabled = false;
            Invoke("DelayedInvoke", SetupPanel.LevelSettings.GhostStartedCooldown);

        }

    }

    void DelayedInvoke()
    {
        GhostEvent.Instance.canStart = true;
        Destroy(gameObject);

    }
}
