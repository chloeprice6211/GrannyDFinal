using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialSwithcer : FuseboxSwitcher
{
    public bool tip;
    [SerializeField] TutorialMessage tutorial;

    public override void TurnOn()
    {
        if(tip == true)
        {
            tutorial.ShowFuseboxObjective();
        }
        tip = true;
        base.TurnOn();
    }


}
