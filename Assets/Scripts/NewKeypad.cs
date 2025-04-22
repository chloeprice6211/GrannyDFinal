using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NewKeypad : FocusObject, IGenerable
{
    [SyncVar] public string combinationToUnlock;
    public string currentCombination;

    [SerializeField] protected TextMeshProUGUI passcodeTextField;

    [SerializeField] protected GameObject objectToUnlock;

    [SerializeField] protected AudioSource audioSource;
    [SerializeField] protected List<AudioClip> keyPressAudioClips;
    [SerializeField] protected AudioClip wrongCombinationAudioClip;

    public bool requireEnter;
    public bool hasPasscodeTextField;
    bool _hasBeenUnlocked;

    protected IUnlockable iObjectToUnlock;

    [SerializeField] GameObject receivePasswordObj;
    IReceivePassword _iReceivePassObj;


    private void Start()
    {
        objectToUnlock.TryGetComponent(out iObjectToUnlock);
    }

    #region add digit
    public virtual void AddDigit(char digit)
    {
        AddDigitCmd(digit);
    }

    [Command (requiresAuthority = false)]
    public void AddDigitCmd(char digit)
    {
        if (currentCombination.Length == combinationToUnlock.Length && requireEnter)
            return;

        
        AddDigitRpc(digit);
    }

    [ClientRpc]
    public void AddDigitRpc(char digit)
    {
        currentCombination += digit;

        if (hasPasscodeTextField && !_hasBeenUnlocked)
            passcodeTextField.text += '*';

        if (currentCombination.Length == combinationToUnlock.Length && !requireEnter)
        {
            Verify();
        }

        audioSource.PlayOneShot(keyPressAudioClips[Random.Range(0, keyPressAudioClips.Count)]);
    }

    #endregion

    #region interface implementations

    public void ApplyGeneratedCode(string code)
    {
        combinationToUnlock = code;
    }

    public void ShowGeneratedCode()
    {
        if(receivePasswordObj.TryGetComponent(out _iReceivePassObj))
        {
            _iReceivePassObj.DisplayPassword(combinationToUnlock);
        }
    }

    #endregion

    public void EraseCombinationCmd()
    {
        currentCombination = string.Empty;

        if (hasPasscodeTextField)
            passcodeTextField.text = string.Empty;
    }

    public virtual void Verify()
    {
        if (_hasBeenUnlocked)
            return;

        if(currentCombination == combinationToUnlock)
        {
            if (iObjectToUnlock != null)
            {
                if (hasPasscodeTextField)
                {
                    passcodeTextField.text = "SUCCESS";
                    passcodeTextField.alignment = TextAlignmentOptions.Center;
                }
                    

                iObjectToUnlock.Unseal();
                _hasBeenUnlocked = true;
            }
        }
        else
        {
            EraseCombinationCmd();
            audioSource.PlayOneShot(wrongCombinationAudioClip, 0.1f);
        }
    }

    [Command (requiresAuthority = false)]
    void VerifyCmd()
    {
        VerifyRpc();
    }
    [ClientRpc]
    void VerifyRpc()
    {
        Verify();
    }

    public void VerifyClientServer()
    {
        VerifyCmd();
    }
}
