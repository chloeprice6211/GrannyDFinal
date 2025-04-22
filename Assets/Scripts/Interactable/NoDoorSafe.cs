using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoDoorSafe : MonoBehaviour, IInteractableRpc
{
    [SerializeField] Animation doorUnlockAnimation;
    [SerializeField] AudioClip unlockSound;
    [SerializeField] Collider _collider;

    public bool isUnlocked;

    public void Unlock()
    {
        if (isUnlocked) 
        {
            return;
        }
        isUnlocked = true;
        _collider.enabled = false;

        AudioSource.PlayClipAtPoint(unlockSound, transform.position);
        doorUnlockAnimation.Play();
    }

    public void Interact(NetworkPlayerController owner)
    {
        if (!owner.hasAuthority) return;
        UIManager.Instance.Message("noDoorSafe", "noDoorSafeUnlock_A");
    }
}
