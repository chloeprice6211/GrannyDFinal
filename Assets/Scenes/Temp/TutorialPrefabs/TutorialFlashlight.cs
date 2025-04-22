using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialFlashlight : Flashlight
{
    [SerializeField] TutorialMessage tutorialMessage;
    bool hasPressed;
    [SerializeField] TutorialNote tutorialNote;

    public override void Interact(NetworkPlayerController owner)
    {
        base.Interact(owner);
    }

    public override void SwitchFlashlight(bool playSound = false)
    {
        base.SwitchFlashlight();

        if (!hasPressed)
        {
            tutorialMessage.ShowTutorialMessage("Lots of useful notes are scattered throughout a house. Go on and grab a note on a table. Once you read it, you can collect the note by pressing [R].");
            hasPressed = true;
            tutorialNote.gameObject.layer = 7;
        }
       
    }
}
