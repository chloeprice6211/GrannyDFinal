using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TutorialNote : Note
{
    [SerializeField] TutorialMessage tutorial;
    public override void CollectNote(InputAction.CallbackContext context)
    {
        base.CollectNote(context);
        tutorial.ShowJournalObjective();

    }
}
