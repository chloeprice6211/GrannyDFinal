using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QrScannerApplication : MobileApplication
{
    [SerializeField] Camera phoneCamera;
    [SerializeField] BrowserApplication browser;
    [SerializeField] GameObject link;

    private Ray _ray;
    private bool _didHit;
    private RaycastHit _impactedObject;
    private bool _isShown;

    public float interactRange = 2;
    [SerializeField] LayerMask layerMask;

    IEnumerator myRoutine;

    private void Start()
    {
        myRoutine = QRRoutine();
    }

    public override void LaunchApplication()
    {
        base.LaunchApplication();
        phoneCamera.enabled = true;
        //myRoutine = QRRoutine();
        StartCoroutine(myRoutine);
    }

    public override void CloseApplication()
    {
        base.CloseApplication();
        phoneCamera.enabled = false;
        StopCoroutine(myRoutine);
        
    }

    public void OnLinkClick()
    {
        Debug.Log("clicked link");
        CmdLink();
    }

    [Command(requiresAuthority = false)]
    void CmdLink()
    {
        RpcClick();
    }
    [ClientRpc]
    void RpcClick()
    {
        StartCoroutine(LaunchBrowserRoutine());
    }

    IEnumerator QRRoutine()
    {
        while (true)
        {
            _ray = phoneCamera.ViewportPointToRay(new Vector3(.5f, .5f, 0));
            _didHit = Physics.Raycast(_ray, out _impactedObject, interactRange, layerMask);

            if (_didHit)
            {
                if (!link.activeInHierarchy)
                {
                    link.SetActive(true);
                }
                    _isShown = true;
            }
            else if (_isShown == true)
            {
                _isShown = false;
                UIManager.Instance.HideInteractOption();
            }


            yield return null;
        }
    }

    IEnumerator LaunchBrowserRoutine()
    {
        s_canAppLaunch = false;
        CloseApplication();
        yield return new WaitForSeconds(0.7f);
        s_canAppLaunch = true;
        browser.LaunchApplication();
        s_canAppLaunch = false;
        browser.hiddenTab.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        s_canAppLaunch = true;

    }


}
