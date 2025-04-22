using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GarageDoor : Mirror.NetworkBehaviour
{
    public int collisionsAmount = 3;

    public GameObject garageParent;
    [SerializeField] Car car;
    [SerializeField] FixedJoint parentFixedJoint;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip collisionSound;
    public GameObject environment;
    public AudioSource carRain;

    public Rigidbody garageParentRB;
    public Rigidbody thisRB;
    public bool isSealed;
    public bool isUnlocked;

    [SerializeField] Animation gateOpenAnimation;

    [SerializeField] AnimationClip openClip;
    [SerializeField] AnimationClip closeClip;

    private void Start()
    {
        garageParentRB = garageParent.GetComponent<Rigidbody>();
        thisRB = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        //if (collision.gameObject.tag == "Car")
        //{
        //    if (!isUnlocked)
        //    {
        //        UIManager.Instance.Message("I won't be able to break the gate until it's unlocked.");
        //        car.PushCarBack();
        //        AudioSource.PlayClipAtPoint(collisionSound, transform.position);

        //        return;
        //    }

        //    //messages
        //    switch (collisionsAmount)
        //    {
        //        case 3: UIManager.Instance.Message("Gotta try this again."); break;
        //        case 2: UIManager.Instance.Message("It's working! Keep it up!"); break;
        //        case 1: UIManager.Instance.Message("Almost there..."); break;
        //    }

        //    if(collisionsAmount == 1)
        //    {
        //        car.PushCarBack();
        //        garageParentRB.isKinematic = false;
        //        garageParentRB.mass = .01f;
        //        thisRB.mass = .01f;

        //        environment.SetActive(true);

        //        collisionsAmount--;
        //    }
        //    else if(collisionsAmount > 1)
        //    {
        //        car.PushCarBack();
        //        collisionsAmount--;
        //    }
        //    else if(collisionsAmount == 0)
        //    {
        //        carRain.Play();
        //    }

        //    //AudioSource.PlayClipAtPoint(collisionSound, transform.position);

        //}
    }

    public void PlayImpactSound()
    {
        audioSource.PlayOneShot(collisionSound);
    }

    public void Open()
    {
        if(gateOpenAnimation == null)
        {
            return;
        }
        gateOpenAnimation.Play(openClip.name);
    }

}
