using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableItem : NetworkBehaviour, IGhostInteractable
{
    private Rigidbody rb;
    [SerializeField] AudioClip throwSound;
    [SerializeField] AudioClip impactClip;

    int _collisionsCount;
    bool _canMakeSound;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void PerformGhostInteraction()
    {
        PushItem();
    }

    private void PushItem()
    {
        rb.isKinematic = false;
        rb.AddForce(new Vector3(2, 2, 1), ForceMode.Impulse);
        AudioSource.PlayClipAtPoint(throwSound, transform.position, .5f);

        _collisionsCount = 0;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!_canMakeSound)
        {
            Invoke("EnableCollideSound", .5f);
            return;
        }
        if (_collisionsCount != 0) return;

        AudioSource.PlayClipAtPoint(impactClip, transform.position);

        _collisionsCount++;
        _canMakeSound = false;
    }
    void EnableCollideSound()
    {
        _canMakeSound = true;
    }
}
