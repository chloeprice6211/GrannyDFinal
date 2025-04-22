using Mirror;
using Steamworks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class DesktopApplication : Mirror.NetworkBehaviour
{
    [SerializeField] Animation appAnimation;
    [SerializeField] Laptop laptop;
    public NewLaptop newLaptop;

    [SerializeField] AnimationClip appOpeningClip;
    [SerializeField] AnimationClip appClosingClip;

    [SerializeField] AudioClip touchpadClickSound;
    public OperatingSystem OS;


    #region launch app c/s

    public virtual void LaunchApplication()
    {
        LaunchApplicationCommand();
        if(gameObject.name == "Dos1Picture" || gameObject.name == "Dos2Picture" || gameObject.name == "Dos3Picture" || gameObject.name == "Dos4Picture")
        {
            SteamUserStats.SetAchievement("DOGGIE");
            SteamUserStats.StoreStats();
        }   
    }

    [Command (requiresAuthority = false)]
    private void LaunchApplicationCommand()
    {
        LaunchApplicationRpc();
    }
    [ClientRpc]
    private void LaunchApplicationRpc()
    {
        if (laptop != null)
        {
            laptop.currentApplication = gameObject.GetComponent<DesktopApplication>();
            
        }
        else if (newLaptop != null)
        {
            newLaptop.currentApplication = gameObject.GetComponent<DesktopApplication>();
        }
        else
        {
            OS.currentApplication = this;
        }

        //OS.currentApplication = this;
        
        appAnimation.Play(appOpeningClip.name);
        AudioSource.PlayClipAtPoint(touchpadClickSound, transform.position);
    }

    #endregion

    #region close app c/s

    public virtual void CloseApplication()
    {
        CloseApplicationCommand();
    }

    [Command(requiresAuthority = false)]
    private void CloseApplicationCommand()
    {
        CloseApplicationRpc();
    }
    [ClientRpc]
    private void CloseApplicationRpc()
    {
        if (laptop != null)
        {
            laptop.currentApplication = null;
        }
        else if (newLaptop != null)
        {
            newLaptop.currentApplication = null;
        }
        else
        {
            OS.currentApplication = null;
        }

        //OS.currentApplication = null;

        appAnimation.Play(appClosingClip.name);
        AudioSource.PlayClipAtPoint(touchpadClickSound, transform.position);
    }

    #endregion

}
