 using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;

public class PrefsSettings : MonoBehaviour
{
    public static float s_mainMenuAmbientCap;
    public static int s_graphics;
    public static int s_vsync;
    public static float s_fov;
    public static int s_tutorial;
    public static float s_volume;

    public static float s_voiceVolume;
    public static int s_hasSubs;
    public static string s_voiceLanguage;
    public static string s_subsLanguage;
    public static string s_language;

    public static float s_sens;
    public static float s_maxSens = 0.085f;

    public static float s_postExposureMax = -1.25f;
    public static float s_gamePostExposureMax = -1.55f;
    public static float s_postExposure;

    private void Awake()
    {
        if (!PlayerPrefs.HasKey("volume"))
        {
            ApplyDefault();
        }
        else
        {
            ApplySettings(
                PlayerPrefs.GetFloat("volume"),
                PlayerPrefs.GetInt("graphics"),
                PlayerPrefs.GetFloat("sens"),
                PlayerPrefs.GetFloat("brightness"),
                PlayerPrefs.GetInt("vsync"),
                PlayerPrefs.GetFloat("fov"),
                PlayerPrefs.GetFloat("voiceVolume"),
                1,
                PlayerPrefs.GetString("voiceLang"),
                PlayerPrefs.GetString("subLang"),
                PlayerPrefs.GetString("language")
                );
        }
    }

    public static void ApplySettings(float volume, int graphics, float sens,
        float exposure, int vsync, float fov,
        float voiceVol, int hasSubs, string voiceLang, string subLang, string language)
    {
        //Debug.Log(voiceVol);

        s_volume = volume;
        s_sens = sens;
        s_postExposure = exposure / 100;
        s_vsync = vsync;
        s_graphics = graphics;
        s_fov = (float)50;
        s_tutorial = 1;

        s_voiceVolume = voiceVol / 100;
        s_hasSubs = hasSubs;
        s_voiceLanguage = voiceLang;
        s_subsLanguage = subLang;
        s_language = language;

        ApplyFramerate(vsync);
        ApplyGraphics(graphics);
        ApplyLanguage(language);
        ApplyVolume(volume);
    }

    public static void ApplyDefault()
    {
        PlayerPrefs.SetFloat("volume", 100);
        PlayerPrefs.SetFloat("sens", 1);
        PlayerPrefs.SetInt("graphics", 5);
        PlayerPrefs.SetFloat("brightness", 0);
        PlayerPrefs.SetInt("vsync", 0);
        PlayerPrefs.SetInt("tip", 0);
        PlayerPrefs.SetFloat("fov", 50);
        PlayerPrefs.SetFloat("tutorial", 0);

        PlayerPrefs.SetFloat("voiceVolume", 70);
        PlayerPrefs.SetInt("hasSubs", 1);
        PlayerPrefs.SetString("subLang", "en");
        PlayerPrefs.SetString("voiceLang", "en");

        string _interfaceLanguageId;
        string _voiceLanguageId;

        //interface lang
        switch (Application.systemLanguage)
        {
            case SystemLanguage.English:
                _interfaceLanguageId = "en";
                break;
            case SystemLanguage.Ukrainian:
                _interfaceLanguageId = "uk";
                break;
            default:
                _interfaceLanguageId = "en";
                break;
        }

        //voice lang
        switch (Application.systemLanguage)
        {
            case SystemLanguage.English:
                _voiceLanguageId = "en";
                break;
            case SystemLanguage.Ukrainian:
                _voiceLanguageId = "uk";
                break;
            default:
                _voiceLanguageId = "en";
                break;
        }

        ApplySettings(100,4, 1, 0, 0, 50,
            70, 1, _voiceLanguageId, _interfaceLanguageId, _interfaceLanguageId);
    }

    public static void ApplyFramerate(int rate)
    {
        QualitySettings.vSyncCount = rate;
    }

    public static void ApplyGraphics(int level)
    {
        QualitySettings.SetQualityLevel(level, false);
    }

    public static void ApplyVolume(float volume){
        AudioListener.volume = volume / 100;
    }

    private static void ApplyLanguage(string language)
    {
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.GetLocale(language);
    }
}
