using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrowserApplication : MobileApplication
{
    public GameObject hiddenTab;
    public GameObject noConnectionTab;

    public override void LaunchApplication()
    {
        base.LaunchApplication();
        Debug.Log("launched");
    }

    public void OnRetryClick()
    {
        if (phone.hasWifi)
        {
            RetryCmd();
        }
    }

    [Command (requiresAuthority = false)]
    void RetryCmd()
    {
        RetryRpc();
    }
    [ClientRpc]
    void RetryRpc()
    { 
        noConnectionTab.SetActive(false);
    }

    public void OnRouterOff()
    {
        noConnectionTab.SetActive(true);
    }
}
