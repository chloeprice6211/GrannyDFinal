
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICheckBox : MonoBehaviour
{
    [SerializeField] Image filledImage;
    [SerializeField] Sprite filledImageSprite;
    [SerializeField] SettingsPanel panel;
    public bool isChecked;
    public string prefKey;
    private void OnEnable()
    {
        if(PlayerPrefs.GetInt(prefKey) == 1){
            Activate();
            isChecked = true;
        }
        else
        {
            Deactivate();
            isChecked = false;
        }
    }

    public void OnClick()
    {
        Switch();
    }

    void Switch()
    {
        isChecked = !isChecked;
        

        if (isChecked)
        {
            Activate();
        }
        else
        {
            Deactivate();
        }
    }

    public void Activate()
    {
        filledImage.enabled = true;
        //panel._vsyncPrefValue = 1;
    }

    public void Deactivate()
    {
        filledImage.enabled = false;
        //panel._vsyncPrefValue = 0;
    }
}
