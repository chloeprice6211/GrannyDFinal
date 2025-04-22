using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialDoor : Door
{
    [SerializeField] TutorialMessage tutorial;
    [SerializeField] TutorialSmartphone smarthpone;
    public override void UnlockDoor(Item key)
    {
        base.UnlockDoor(key);
        gameObject.layer = 0;
        tutorial.count = 7;

        tutorial.ShowTutorialMessage("Very good. Let's not open it for now. Go ahead and grab a phone on the PC table.");
        smarthpone.gameObject.layer = 7;
        smarthpone.outlineShader.enabled = true;
    }
}
