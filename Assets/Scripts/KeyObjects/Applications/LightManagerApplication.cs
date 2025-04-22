using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;

public class LightManagerApplication : MobileApplication
{
    [SerializeField] Fusebox fusebox;

    //UI
    [SerializeField] TextMeshProUGUI statusText;
    [SerializeField] Button unlockButton;
    [SerializeField] public TextMeshProUGUI buttonText;

    private void Start()
    {
        OnFuseboxRangeExit();
        buttonText.text = LocalizationSettings.StringDatabase.GetLocalizedString("inGameItems", "fuseboxButtonLocked");
        LocalizationSettings.SelectedLocaleChanged += LocalizationSettings_SelectedLocaleChanged;
    }

    private void LocalizationSettings_SelectedLocaleChanged(UnityEngine.Localization.Locale obj)
    {
        buttonText.text = fusebox.isLocked ? LocalizationSettings.StringDatabase.GetLocalizedString("inGameItems", "fuseboxButtonLocked") :
            LocalizationSettings.StringDatabase.GetLocalizedString("inGameItems", "fuseboxButtonUnocked");
    }

    public override void LaunchApplication()
    {
        base.LaunchApplication();
        DisplayInternalTip();
    }

    public void OnUnlockClick()
    {
        fusebox.LockUnlockCommand(this);
    }

    public void OnFuseboxRangeEnter()
    {
        unlockButton.interactable = true;
        statusText.text = LocalizationSettings.StringDatabase.GetLocalizedString("inGameItems", "fuseboxStatusInRange");
        statusText.color = Color.green;

    }

    public void OnFuseboxRangeExit()
    {
        unlockButton.interactable = false;
        statusText.text = LocalizationSettings.StringDatabase.GetLocalizedString("inGameItems", "fuseboxStatusOutOfRange");
        statusText.color = Color.red;

    }
}
