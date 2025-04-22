using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class MainMenuMonitor : MonoBehaviour
{
    [SerializeField] AudioClip turnOnSound;
    [SerializeField] AudioSource aSource;

    [SerializeField] Canvas ui;
    [SerializeField] Light lightSource;
    [SerializeField] Animation startGlitchAnimation;

    [SerializeField] TextMeshProUGUI titleText;

    //menu components
    [SerializeField] GameObject mainPanel;
    [SerializeField] GameObject settingsPanel;
    [SerializeField] GameObject bugReportPanel;
    public List<GameObject> objectsToDisplay;

    [SerializeField] Animation buttonAnimation;
    [SerializeField] Animation anotherAnimation;

    void Start()
    {
        StartCoroutine(StartRoutine());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator StartRoutine()
    {
        yield return new WaitForSeconds(2f);
        buttonAnimation.Play();
        anotherAnimation.Play();
        TurnOn();
        startGlitchAnimation.Play();
        ui.enabled = true;

        yield return new WaitForSeconds(1f);

        foreach(GameObject obj in objectsToDisplay)
        {
            obj.SetActive(true);
        }
        

        mainPanel.SetActive(true);
    }

    public void TurnOn()
    {
        lightSource.enabled = true;
        aSource.PlayOneShot(turnOnSound);
    }

    #region button handlers

    public void OnSettingsClick()
    {
        mainPanel.SetActive(false);
        settingsPanel.SetActive(true);
        titleText.text = LocalizationSettings.StringDatabase.GetLocalizedString("mainMenu", "settingsSelection");
    }

    public void OnReturnFromSettingsClick()
    {
        mainPanel.SetActive(true);
        titleText.text = LocalizationSettings.StringDatabase.GetLocalizedString("mainMenu", "locationSelection");
        //settingsPanel.SetActive(false);
    }

    public void OnReportBugClick()
    {
        mainPanel.SetActive(false);
        bugReportPanel.SetActive(true);
        //titleText.text = "REPOT A BUG";
    }

    public void OnQuitClick()
    {
        Application.Quit();
    }

    public void OnDiscordClick()
    {
        Application.OpenURL("https://discord.gg/V7CxpnhxCx");
    }

    #endregion
}
