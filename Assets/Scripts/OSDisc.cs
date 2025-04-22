using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OSDisc : UtilityItem
{
    [SerializeField] AudioClip insertClip;

    public override void OnInsert()
    {
        base.OnInsert();
        AudioSource.PlayClipAtPoint(insertClip, transform.position);
        GetComponent<Collider>().enabled = false;
    }

}

