using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TutorialSmartphone : Smartphone
{
    [SerializeField] TutorialMessage tutorial;
    bool hasused;
    bool hastaken;

    public override void TakeItem(NetworkPlayerController item)
    {
        base.TakeItem(item);

        if (!hastaken)
        {
            tutorial.ShowTutorialMessage("You can access any device such as phone, laptops etc by pressing [R]. You can find here useful information and interact with it's applications. Go ahead and use it!");
            hastaken = true;
        }
        outlineShader.enabled = false;
    }

    public override void UseDevice(InputAction.CallbackContext context)
    {
        base.UseDevice(context);
        if (!hasused)
        {
            hasused = true;
            tutorial.ShowPhoneObjective();
        }
        
    }
}
