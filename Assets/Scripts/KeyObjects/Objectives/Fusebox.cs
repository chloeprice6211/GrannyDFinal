using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization.Settings;

public class Fusebox : NetworkBehaviour, IGhostInteractable
{
    public bool isLocked = true;
    Collider fuseboxCollider;

    [SerializeField] Smartphone phone;
    [SerializeField] AudioClip unlockSound;
    [SerializeField] GameObject dispayPanel;
    [SerializeField] AudioClip blackoutSound;
    [SerializeField] AudioSource fuseboxAudio;

    [SerializeField] Renderer displayScreenMaterial;
    [SerializeField] Material lockedScreenMat;
    [SerializeField] Material unlockedScreenMat;

    public List<FuseboxSwitcher> switchers;

    public UnityEvent OnFuseboxRangeEnter;
    public UnityEvent OnFuseBoxRangeExit;

    public bool hasRecentlyBeenOff;


    private void Start()
    {
        fuseboxCollider = GetComponent<Collider>();
    }


    #region Lock/Unlock client/server

    [Command (requiresAuthority = false)]
    public void LockUnlockCommand(LightManagerApplication app)
    {
        LockUnlockRpc(app);
    }
    [ClientRpc]
    private void LockUnlockRpc(LightManagerApplication app)
    {
        LockUnlockFusebox(app);
    }

    public void LockUnlockFusebox(LightManagerApplication app)
    {
        AudioSource.PlayClipAtPoint(unlockSound, transform.position);
        isLocked = !isLocked;

        if (isLocked)
        {
            app.buttonText.text = LocalizationSettings.StringDatabase.GetLocalizedString("inGameItems", "fuseboxButtonLocked");
            displayScreenMaterial.material = lockedScreenMat;
        }
        else
        {
            app.buttonText.text = LocalizationSettings.StringDatabase.GetLocalizedString("inGameItems", "fuseboxButtonUnocked");
            displayScreenMaterial.material = unlockedScreenMat;
        }
    }

    #endregion

    [ClientRpc]
    public void PerformGhostInteraction()
    {
        if (hasRecentlyBeenOff) return;

        fuseboxAudio.PlayOneShot(blackoutSound);

        foreach(FuseboxSwitcher switcher in switchers)
        {
            if (switcher.isOn)
            {
                switcher.TurnOff();
            }
        }

        hasRecentlyBeenOff = true;

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Smartphone")
        {
            OnFuseboxRangeEnter?.Invoke();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.name == "Smartphone")
        {
            OnFuseBoxRangeExit?.Invoke();
        }

    }

}
