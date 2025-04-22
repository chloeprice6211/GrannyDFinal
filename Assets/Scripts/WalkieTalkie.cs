using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

public class WalkieTalkie : Device, IGenerable, IInsertable
{
    [SerializeField] AudioClip batteryInstertSound;

    [Header("Audio")]
    [SerializeField] AudioClip radioStartClip;
    [SerializeField] List<AudioClip> morseCodeAudioClips;
    [SerializeField] List<AudioClip> miscClips;
    [SerializeField] AudioSource radioSource;
    [SerializeField] AudioSource mushSource;
    AudioClip morseCodeProperClip;

    [Header("Renderer")]

    [SerializeField] Renderer _renderer;
    [SerializeField] Material activeMat;
    [SerializeField] Material inactiveMat;

    [Header("Attributes")]

    [SerializeField] GameObject displayHolder;
    [SerializeField] NewKeypad keypad;

    public List<TextMeshProUGUI> channelsTextFields;
    public List<GameObject> correts;

    public int batteries;
    public bool isPowered;
    public bool isOn;

    [SyncVar] string properCode;

    TextMeshProUGUI currentChannelTextField;

    int selectedChannelIndex = 0;

    private void Start()
    {
        currentChannelTextField = channelsTextFields[0];
    }

    #region button handlers

    public void AdjustChannel(int value)
    {
        if (!isOn) return;

        AdjustChannelCmd(value);

      
    }

    public void ChangeChannel()
    {
        if (!isOn) return;

        ChangeChannelCmd();

    }

    void SetChannelAudio(int firstChannel, int secondChannel)
    {
        if(firstChannel == 410 && secondChannel == 112)
        {
            AudioSource.PlayClipAtPoint(radioStartClip, transform.position, .5f);

            radioSource.volume = 0.2f;
            radioSource.clip = miscClips[0];
            radioSource.Stop();
            radioSource.Play();
        }
        else if(firstChannel == 530 && secondChannel == 112)
        {
            AudioSource.PlayClipAtPoint(radioStartClip, transform.position, .5f);

            radioSource.volume = 0.2f;
            radioSource.clip = miscClips[1];
            radioSource.Stop();
            radioSource.Play();
        }
        else if (firstChannel == 350 && secondChannel == 114)
        {
            AudioSource.PlayClipAtPoint(radioStartClip, transform.position, .5f);

            radioSource.volume = 0.2f;
            radioSource.clip = miscClips[2];
            radioSource.Stop();
            radioSource.Play();
        }
        else if(firstChannel == 440 && secondChannel == 122)
        {
            AudioSource.PlayClipAtPoint(radioStartClip, transform.position, .5f);

            radioSource.volume = 0.5f;
            radioSource.clip = morseCodeProperClip;
            radioSource.Stop();
            radioSource.Play();
        }
        else
        {
            radioSource.volume = 0;
        }
    }

    public void TurnOn()
    {
        if (!isPowered)
        {
            UIManager.Instance.Message("twoBatteries", "twoBatteries_A");
            return;
        }

        TurnOnCmd();
    }

    public void TurnOff()
    {
        TurnOffCmd();
    }

    #endregion

    #region client server

    [Command (requiresAuthority = false)]
    void AdjustChannelCmd(int value)
    {
        AdjustChannelRpc(value);
    }
    [ClientRpc]
    void AdjustChannelRpc(int value)
    {
        if (value < 0)
        {
            value = selectedChannelIndex == 0 ? -10 : -2;
        }
        else
        {
            value = selectedChannelIndex == 0 ? 10 : 2;
        }


        int currentChannelValue = Convert.ToInt32(currentChannelTextField.text);
        int newChannelValue = currentChannelValue + value;

        if (newChannelValue >= 700 && value == 10)
        {
            newChannelValue = 400;
        }
        else if (newChannelValue >= 200 && value == 2)
        {
            newChannelValue = 90;
        }
        currentChannelTextField.text = newChannelValue.ToString();

        SetChannelAudio(Convert.ToInt32(channelsTextFields[0].text), Convert.ToInt32(channelsTextFields[1].text));
    }

    [Command (requiresAuthority = false)]
    void TurnOnCmd()
    {
        TurnOnRpc();
    }
    [ClientRpc]
    void TurnOnRpc()
    {
        AudioSource.PlayClipAtPoint(radioStartClip, transform.position, .5f);

        mushSource.Play();

        _renderer.material = activeMat;
        displayHolder.SetActive(true);

        radioSource.Play();

        isOn = true;
    }

    [Command (requiresAuthority = false)]
    void TurnOffCmd()
    {
        TurnOffRpc();
    }
    [ClientRpc]
    void TurnOffRpc()
    {
        mushSource.Stop();

        isOn = false;
        _renderer.material = inactiveMat;
        displayHolder.SetActive(false);
        radioSource.Stop();
    }

    [Command (requiresAuthority = false)]
    void ChangeChannelCmd()
    {
        ChangeChannelRpc();
    }
    [ClientRpc]
    void ChangeChannelRpc()
    {
        int index = selectedChannelIndex == 0 ? 1 : 0;

        currentChannelTextField = channelsTextFields[index];

        correts[0].gameObject.SetActive(false);
        correts[1].gameObject.SetActive(false);
        correts[index].gameObject.SetActive(true);

        selectedChannelIndex = index;
    }


    #endregion

    public void ApplyGeneratedCode(string code)
    {
        //switch (code)
        //{
        //    case "SNARGU": morseCodeProperClip = morseCodeAudioClips[0]; break;
        //    case "UNGARS": morseCodeProperClip = morseCodeAudioClips[1]; break;
        //    case "NURASG": morseCodeProperClip = morseCodeAudioClips[2]; break;
        //    case "AUGSNR": morseCodeProperClip = morseCodeAudioClips[3]; break;
        //    case "SURANG": morseCodeProperClip = morseCodeAudioClips[4]; break;
        //}

        properCode = code;
        keypad.ApplyGeneratedCode(code);
    }

    public void ShowGeneratedCode()
    {
        switch (properCode)
        {
            case "SNARGU": morseCodeProperClip = morseCodeAudioClips[0]; break;
            case "UNGARS": morseCodeProperClip = morseCodeAudioClips[1]; break;
            case "NURASG": morseCodeProperClip = morseCodeAudioClips[2]; break;
            case "AUGSNR": morseCodeProperClip = morseCodeAudioClips[3]; break;
            case "SURANG": morseCodeProperClip = morseCodeAudioClips[4]; break;
        }
    }

    public void Insert(UtilityItem itemToInsert, NetworkPlayerController owner)
    {
        Debug.Log("insert called");
        //UIManager.Instance.Message("remoteInsertBattery", "shouldWorkV2_A");
        InsertCmd(itemToInsert);
    }
    [Command(requiresAuthority = false)]
    void InsertCmd(UtilityItem itemToInsert)
    {
        InsertRpc(itemToInsert);
    }
    [ClientRpc]
    void InsertRpc(UtilityItem item)
    {
        batteries++;

        if (batteries == 1)
        {
            UIManager.Instance.Message("oneToGo", "oneToGo_A");
        }
        else if (batteries == 2)
        {
            UIManager.Instance.Message("workProperly", "shouldWorkV2_A");
            isPowered = true;
        }

        AudioSource.PlayClipAtPoint(batteryInstertSound, transform.position);
        Destroy(item.transform.parent.gameObject);
    }
}
