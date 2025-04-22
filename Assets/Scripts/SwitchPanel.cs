using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class SwitchPanel : NetworkBehaviour, IUnlockable, IGenerable
{
    public bool isKeyInstalled;
    [SerializeField] NoDoorSafe safe;

    [Header("UI")]
    [SerializeField] TextMeshProUGUI bigButtonText;
    [SerializeField] TextMeshProUGUI southWestButtonText;
    [SerializeField] TextMeshProUGUI northWestButtonText;
    [SerializeField] TextMeshProUGUI southEastButtonText;
    [SerializeField] TextMeshProUGUI northEastButtonText;
    [SerializeField] TextMeshProUGUI eastSwitcherText;
    [SerializeField] TextMeshProUGUI westSwitcherText;

    public bool[] currentSwitchersArray = new bool[7];
    public bool[] correctSwitchersArray = new bool[7];
    [SyncVar] public string values;
    public string[] names = new string[7];

    string localizedOn = "ON";
    string localizedOff = "OFF";

    bool _hasBeenUnlocked;

    [SerializeField] AudioClip successClip;

    private void Awake()
    {
        currentSwitchersArray = new bool[7];
        names = new string[7];

        localizedOn = LocalizationSettings.StringDatabase.GetLocalizedString("inGameItems", "on");
        localizedOff = LocalizationSettings.StringDatabase.GetLocalizedString("inGameItems", "off");

        LocalizationSettings.SelectedLocaleChanged += LocalizationSettings_SelectedLocaleChanged;
    }

    private void LocalizationSettings_SelectedLocaleChanged(UnityEngine.Localization.Locale obj)
    {
        ShowCodes();
    }

    public bool Check()
    {
        if (!isKeyInstalled) return false;

        for (int a = 0; a < 7; a++)
        {
            if (currentSwitchersArray[a] != correctSwitchersArray[a])
            {
                return false;
            }
        }

        return true;
    }

    public void ActivateLever()
    {
        if (!isKeyInstalled)
        {
            UIManager.Instance.Message("leverFail", "leverFail_A");
        }

        if (Check() && !_hasBeenUnlocked)
        {
            _hasBeenUnlocked = true;
            UIManager.Instance.Message("leverSuccess", "leverSuccess_A");
            AudioSource.PlayClipAtPoint(successClip, transform.parent.transform.position);
            safe.Unlock();
        }
    }

    public void ActivateSwitcher(int index)
    {
        currentSwitchersArray[index] = !currentSwitchersArray[index];
    }

    public void ShowCodes()
    {
        localizedOn = LocalizationSettings.StringDatabase.GetLocalizedString("inGameItems", "on");
        localizedOff = LocalizationSettings.StringDatabase.GetLocalizedString("inGameItems", "off");
        NameArray();

        bigButtonText.text = LocalizationSettings.StringDatabase.GetLocalizedString("inGameItems", "big") + names[0];

        northWestButtonText.text = LocalizationSettings.StringDatabase.GetLocalizedString("inGameItems","northWest") + names[1];
        southWestButtonText.text = LocalizationSettings.StringDatabase.GetLocalizedString("inGameItems", "southWest") + names[2];
        northEastButtonText.text = LocalizationSettings.StringDatabase.GetLocalizedString("inGameItems", "northEast") + names[3];
        southEastButtonText.text = LocalizationSettings.StringDatabase.GetLocalizedString("inGameItems", "southEast") + names[4];

        westSwitcherText.text = LocalizationSettings.StringDatabase.GetLocalizedString("inGameItems", "westSwitch") + names[5];
        eastSwitcherText.text = LocalizationSettings.StringDatabase.GetLocalizedString("inGameItems", "eastSwitch") + names[6];
    }

    public void ApplyGeneratedCode(string _values)
    {
        values = "";

        bool isTrue;

        for (int a = 0; a < 7; a++)
        {
            isTrue = Random.Range(0, 2) == 1 ? true : false;

            if (isTrue)
            {
                values += '1';
            }
            else
            {
                values += '0';
            }
        }
    }



    public void NameArray()
    {
        for (int a = 0; a < 7; a++)
        {
            correctSwitchersArray[a] = values[a] == '1' ? true : false;
        }

        for (int a = 0; a < 7; a++)
        {
            if (correctSwitchersArray[a] == false)
            {
                names[a] = localizedOff;
            }
            else
            {
                names[a] = localizedOn;
            }
        }
    }

    public void Unseal()
    {
        ActivateLever();
    }

    public void ShowGeneratedCode()
    {
        Debug.Log(values);
        NameArray();

        bigButtonText.text = "BIG = " + names[0];

        northWestButtonText.text = "NORTH WEST = " + names[1];
        southWestButtonText.text = "SOUTH WEST = " + names[2];
        northEastButtonText.text = "NORTH EAST = " + names[3];
        southEastButtonText.text = "SOUTH EAST = " + names[4];

        westSwitcherText.text = "WEST SWITCH = " + names[5];
        eastSwitcherText.text = "EAST SWITCH = " + names[6];
    }
}
