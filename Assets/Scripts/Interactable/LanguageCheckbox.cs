using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class LanguageCheckbox : MonoBehaviour
{
    public string prefsKey;
    string _langId;
    public int currentIndex;

    [SerializeField] SettingsPanel settingsPanel;
    [SerializeField] TextMeshProUGUI flagValueTf; 
    public List<Sprite> flagSprites;
    public List<string> languages;
    public Image flagImg;

    public string languageIndex;

    private void OnEnable()
    {
        Switch(PlayerPrefs.GetString(prefsKey));
        Debug.Log("invoked");
    }

    public void NextItem()
    {
        int _currentIndex = currentIndex == languages.Count - 1 ? 0 : currentIndex + 1;
        currentIndex = _currentIndex;

        flagValueTf.text = languages[currentIndex];
        flagImg.sprite = flagSprites[currentIndex];
        Switch(languages[currentIndex]);

    }
    public void PreviosuItem()
    {
        int _currentIndex = currentIndex == 0 ? languages.Count - 1 : currentIndex - 1;
        currentIndex = _currentIndex;

        flagValueTf.text = languages[currentIndex];
        flagImg.sprite = flagSprites[currentIndex];
        Switch(languages[currentIndex]);
    }

    public void Switch(string localeId)
    {
        switch (localeId)
        {
            case "en":
                flagImg.sprite = flagSprites[0];
                flagValueTf.text = "ENGLISH";
                languageIndex = "en";
                currentIndex = 0;
                break;
            case "uk":
                flagImg.sprite = flagSprites[1];
                flagValueTf.text = "УКРАЇНСЬКА";
                languageIndex = "uk";
                currentIndex = 1;
                break;
        }

        switch (prefsKey)
        {
            case "language":
                settingsPanel._languagePrefsValue = languageIndex;
                break;
            case "voiceLang":
                settingsPanel._voicePrefsValue = languageIndex;
                break;
        }
    }


}
