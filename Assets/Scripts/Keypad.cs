using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Keypad : NetworkBehaviour, IGenerable
{
    [SyncVar] public string properCombination;
    string _currentCombination;
    [SerializeField] Door doorToUnlock;
    [SerializeField] TextMeshProUGUI textField;
    [SerializeField] AudioClip failClip;
    [SerializeField] AudioClip successClip;

    [SerializeField] Renderer _renderer;
    [SerializeField] Material unlockedMat;
    [SerializeField] Light lightSource;

    public void AddDigit(char _digit)
    {
        _currentCombination += _digit;

        if(_currentCombination.Length == 5)
        {
            ApplyCombination();
        }
    }

    public void ApplyGeneratedCode(string code)
    {
        properCombination = code;
    }

    public void ShowGeneratedCode()
    {
        textField.text = properCombination;
    }

    void ApplyCombination()
    {
        if(_currentCombination == properCombination)
        {
            Debug.Log("sucesss");
            AudioSource.PlayClipAtPoint(successClip, transform.position, .5f);

            _renderer.material = unlockedMat;
            lightSource.color = Color.green;

            doorToUnlock.isSealed = false;
            AudioSource.PlayClipAtPoint(successClip, transform.position, .5f);

            //CmdUnlock();
        }
        else
        {
            Debug.Log("lol");
            _currentCombination = string.Empty;
            AudioSource.PlayClipAtPoint(failClip, transform.position, .1f);
        }
    }

    [Command (requiresAuthority = false)]
    void CmdUnlock()
    {
        RpcUnlock();
    }
    [ClientRpc]
    void RpcUnlock()
    {
       
    }
}
