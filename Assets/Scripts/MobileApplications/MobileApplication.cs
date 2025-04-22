using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

abstract public class MobileApplication : Mirror.NetworkBehaviour
{
    [SerializeField] Animation applicationAnimation;
    [SerializeField] protected Smartphone phone;
    
    [TextArea]
    [SerializeField] protected string internalOneTimeTip;
    [SerializeField] protected string internalOneTimeTipAudioKey;
    protected bool _hasInternalTipDisplayed;

    public static bool s_canAppLaunch = true;

    #region launch c/s

    [Command (requiresAuthority = false)]
    private void LaunchApplicationCommand()
    {
        LaunchApplicationRpc();
    }
    [ClientRpc]
    private void LaunchApplicationRpc()
    {
        phone.currentApplication = gameObject.GetComponent<MobileApplication>();
        applicationAnimation.Play("MobileApplicationOpening");
        
    }

    public virtual void LaunchApplication()
    {
        if (!s_canAppLaunch)
        {
            Debug.Log("not");
            return;
        }

        LaunchApplicationCommand();

        s_canAppLaunch = false;
        Invoke("EnableLaunch", 1f);

    }

    void EnableLaunch()
    {
        s_canAppLaunch = true;
    }


    #endregion

    #region close c/s

    [Command (requiresAuthority = false)]
    private void CloseApplicationCommand()
    {
        CloseApplicationRpc();
    }

    [ClientRpc]
    private void CloseApplicationRpc()
    {
        applicationAnimation.Play("MobileApplicationClosing");
        phone.currentApplication = null;
    }

    public virtual void CloseApplication()
    {
        CloseApplicationCommand();
    }

    #endregion

    protected void DisplayInternalTip()
    {
        if (!_hasInternalTipDisplayed)
        {
            UIManager.Instance.Message(internalOneTimeTip, internalOneTimeTipAudioKey);
            _hasInternalTipDisplayed = true;
        }
    }

}
