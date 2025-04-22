using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialKey : Key
{
    [SerializeField] TutorialMessage tutorial;
    [SerializeField] Door door;
    bool hasbeeninspected;
    bool hasbeentaken;

    public override void OnDropItem()
    {
        if (inspectAnimation.isPlaying)
        {
            Debug.Log("clients");
            //ReturnFromInspect();
            inspectAnimation.Stop();
        }

        gameObject.layer = 7;
        AudioSource.PlayClipAtPoint(takeItemSound, transform.position, .5f);
        //if (ObjectSpawnerManager.Instance.isOutlineAllowed) outlineShader.enabled = true;

    }

    public override void TakeItem(NetworkPlayerController owner)
    {
        AudioSource.PlayClipAtPoint(takeItemSound, transform.position, .5f);
        //Inventory.Instance.TakeItem(this, owner);
        gameObject.layer = 13;
        _owner = owner;

        if (!hasbeentaken)
        {
            tutorial.ShowTutorialMessage("Great! You can inspect the item in your hands by pressing [F] to get a more detailed description on it. Go on!");
            hasbeentaken = true;
        }
       
        outlineShader.enabled = false;
    }

    public override void ReturnFromInspect()
    {
        //base.ReturnFromInspect();

        //if (!hasbeeninspected)
        //{
        //    tutorial.ShowTutorialMessage("Alright, now try to unlock the exit door using this key.");
        //    hasbeeninspected = true;
        //}
        
        //door.gameObject.layer = 7;
    }
}
