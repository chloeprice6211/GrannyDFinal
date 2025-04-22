using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsPanel : MonoBehaviour
{   
    [SerializeField] TextMeshProUGUI volumeValue;
    [SerializeField] Slider volumeSlider;

    [SerializeField] TextMeshProUGUI brightnessValueText;
    [SerializeField] Slider brightnessSlider;

    [SerializeField] TextMeshProUGUI graphicsValueText;
    [SerializeField] Slider graphicsSlider;

    [SerializeField] TextMeshProUGUI sensValueText;
    [SerializeField] Slider sensSlider;

    [SerializeField] TextMeshProUGUI foxValueText;
    [SerializeField] Slider fovSlider;

    [SerializeField] TextMeshProUGUI ambientValueText;
    [SerializeField] Slider ambientSlider;

    [SerializeField] TextMeshProUGUI voiceVolumeText;
    [SerializeField] Slider voiceVolumeSlider;

    [SerializeField] MainMenuMonitor mainMenuMonitor;

    [SerializeField] Dropdown graphicsDropdown;

    [SerializeField] UICheckBox vsyncCheckbox;
    [SerializeField] UICheckBox subtitlesCheckbox;

    float _brightnessPrefValue;
    float _fovPrefValue;
    int _graphicsPrefValue;
    float _sensPrefValue;
    public int _vsyncPrefValue;
    float _volumePrefValue;

    int _subtitlesPrefsValue;
    float _voiceVolumePrefsValue;
    public string _languagePrefsValue;
    public string _subsLangPrefsValue;
    public string _voicePrefsValue;

    private void Start()
    {
    }

    private void OnEnable()
    {
        volumeSlider.value = PlayerPrefs.GetFloat("volume");
        OnVolumeSliderValueChanged();

        brightnessSlider.value = PlayerPrefs.GetFloat("brightness");
        OnBrightnessSliderValueChanged();

        graphicsSlider.value = PlayerPrefs.GetInt("graphics");
        OnGraphicsValueChanged();

        fovSlider.value = PlayerPrefs.GetFloat("fov");
        OnFovValueChanged();

        sensSlider.value = PlayerPrefs.GetFloat("sens");
        OnSensSliderValueChanged();

        voiceVolumeSlider.value = PlayerPrefs.GetFloat("voiceVolume");
        OnVoiceVolumeValueChanged();

        OnVsyncValueChanged(0);
        OnSubtitlesValueChanged(0);

    }

    public void OnOkButtonClick()
    {
        if (SceneManager.GetActiveScene().name == "Menu")
        {
            //PlayerPrefs.SetFloat("ambientVolume", _ambientVolumePrefValue);
            mainMenuMonitor.OnReturnFromSettingsClick();
        }
        PlayerPrefs.SetFloat("brightness", _brightnessPrefValue);
        PlayerPrefs.SetInt("graphics", _graphicsPrefValue);
        PlayerPrefs.SetFloat("sens", _sensPrefValue);
        PlayerPrefs.SetInt("vsync", _vsyncPrefValue);
        PlayerPrefs.SetFloat("fov", _fovPrefValue);
        PlayerPrefs.SetFloat("volume", _volumePrefValue);

        PlayerPrefs.SetInt("hasSubs", _subtitlesPrefsValue);
        PlayerPrefs.SetFloat("voiceVolume", _voiceVolumePrefsValue);
        PlayerPrefs.SetString("language", _languagePrefsValue);
        PlayerPrefs.SetString("subLang", _subsLangPrefsValue);
        PlayerPrefs.SetString("voiceLang", _voicePrefsValue);

        Apply();

        gameObject.SetActive(false);

    }

    #region slider handlers

    public void OnBrightnessSliderValueChanged()
    {
        float _value = brightnessSlider.value;
        _brightnessPrefValue = brightnessSlider.value;

        if (_value > 0)
        {
            brightnessValueText.text = "+" + _value.ToString("0.00") + "%";
        }
        else
        {
            brightnessValueText.text = _value.ToString("0.00") + "%";
        }

    }

    public void OnVolumeSliderValueChanged(){
        float _value = volumeSlider.value;
        _volumePrefValue = volumeSlider.value;

        volumeValue.text = _value + "%";
    }

    public void OnFovValueChanged()
    {
        float _value = fovSlider.value;
        _fovPrefValue = fovSlider.value;

        foxValueText.text = (_value + 15).ToString();
    }

    public void OnGraphicsValueChanged()
    {
        float _value = (graphicsSlider.value);
        _graphicsPrefValue = (int)graphicsSlider.value;

        switch (_value)
        {
            case 2:
                graphicsValueText.text = "LOW";
                break;
            case 3:
                graphicsValueText.text = "MID";
                break;
            case 4:
                graphicsValueText.text = "HIGH";
                break;
            case 5:
                graphicsValueText.text = "ULTRA";
                break;
        }
    }

    public void OnSensSliderValueChanged()
    {
        float _value = sensSlider.value;
        _sensPrefValue = sensSlider.value;

        sensValueText.text = _value.ToString("0.00");

    }

    public void OnVsyncValueChanged(int value)
    {
        Debug.Log(vsyncCheckbox.isChecked);
        if (vsyncCheckbox.isChecked)
        {
            _vsyncPrefValue = 1;
        }
        else
        {
            _vsyncPrefValue = 0;
        }
        //_vsyncPrefValue = value;
    }

    public void OnSubtitlesValueChanged(int value)
    {
        Debug.Log(subtitlesCheckbox.isChecked);
        if (subtitlesCheckbox.isChecked)
        {
            _subtitlesPrefsValue = 1;
        }
        else
        {
            _subtitlesPrefsValue = 0;
        }
    }

    public void OnVoiceVolumeValueChanged()
    {
        float _value = voiceVolumeSlider.value;
        _voiceVolumePrefsValue = _value;

        voiceVolumeText.text = _value.ToString("0.00");
    }

    #endregion

    #region language handlers



    #endregion

    #region apply settings

    void Apply()
    {
        Debug.Log(_subtitlesPrefsValue);

        PrefsSettings.ApplySettings(_volumePrefValue,_graphicsPrefValue, _sensPrefValue,
            _brightnessPrefValue, _vsyncPrefValue, _fovPrefValue, _voiceVolumePrefsValue,
            _subtitlesPrefsValue, _voicePrefsValue, _subsLangPrefsValue, _languagePrefsValue);

        if (
            SceneManager.GetActiveScene().name == "SampleScene"
            || SceneManager.GetActiveScene().name == "LVL2"
            || SceneManager.GetActiveScene().name == "TutorialScene"
            || SceneManager.GetActiveScene().name == "LVL3")
        {
            NetworkPlayerController.NetworkPlayer._cameraController.mouseSensitivity = PrefsSettings.s_maxSens * PrefsSettings.s_sens;
            NetworkPlayerController.NetworkPlayer._cameraController._colorGrading.postExposure.value = GameManager.Instance.defaultVolume.GetSetting<ColorGrading>().postExposure.value + (PrefsSettings.s_postExposure * 2);
            NetworkPlayerController.NetworkPlayer._cameraController.virtualCamera.m_Lens.FieldOfView = PrefsSettings.s_fov;
        }
    }

    #endregion
}
