using UnityEngine;

public class PTDoor : MonoBehaviour, IUnlockable
{
    public Animation unlockAnimation;
    public GameObject focusObject;
    public bool Check()
    {
        return true;
    }

    public void Unseal()
    {
        unlockAnimation.Play();
        focusObject.layer = 0;
    }

    
}
