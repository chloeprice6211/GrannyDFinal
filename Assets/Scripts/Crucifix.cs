using Mirror;
using Steamworks;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Crucifix : Item
{
    public float chance;
    [SyncVar] public int uses = 2;
    bool controlsTipAppeared;
    [SerializeField] AudioClip damageSource;

    [SerializeField] GameObject part1;
    [SerializeField] GameObject part2;

    bool wasTaken;


    public override void TakeItem(NetworkPlayerController owner)
    {   
        base.TakeItem(owner);
    }

    [Command (requiresAuthority = false)]
    public void DamageCmd()
    {
        //uses--;
        Damage();
        Damage();
    }

    [ClientRpc]
    public void Damage()
    {
        Debug.Log("USED CRUCIFX");
        uses--;
        Debug.Log(uses);
        AudioSource.PlayClipAtPoint(damageSource, transform.position, 1f);

        if(!Application.isEditor){
            SteamUserStats.SetAchievement("CRUCIFIX_USE");
        SteamUserStats.StoreStats();
        }
        

        if (uses == 1)
        {
            part1.SetActive(false);
        }
        else if(uses == 0)
        {
            part2.SetActive(false);
        }
    }
}
