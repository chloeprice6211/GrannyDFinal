using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;

public class Safe : NetworkBehaviour, IGenerable, IUnlockable
{
    [SerializeField] TextMeshProUGUI codeCombination;
    [SerializeField] EmailLetter letter;
    [SerializeField] TextMeshProUGUI textField;
    [SerializeField] Collider _collider;
    [SerializeField] NewKeypad keypad;
    public bool isLvl2;

    [SerializeField] AudioClip failure;
    [SerializeField] AudioClip opening;

    [SerializeField] GameObject door;

    [SyncVar] public string properCombination;
    [SyncVar] public string combination = string.Empty;

    public void AddCombinationDigit(char digit)
    {
        if(combination.Length < 9)
        {
            if (codeCombination.text == "WRONG" || codeCombination.text == "SUCCESS")
            {
                EraseCombination();
            }

            codeCombination.text += '*';
            combination += digit;
        }

    }

    public void ApplyCombination()
    {
        if(combination == properCombination)
        {
            codeCombination.text = "SUCCESS";
            Invoke("RpcOpenDoor", 1f);
        }
        else
        {
            codeCombination.text = "WRONG";
            this.combination = "";

            AudioSource.PlayClipAtPoint(failure, transform.position, .2f);
        }

    }
    public void EraseCombination()
    {
        combination = "";
        codeCombination.text = "";

    }

    //[Command (requiresAuthority = false)]
    //private void CmdOpenDoor()
    //{
    //    RpcOpenDoor();
    //}
    //[ClientRpc]
    void RpcOpenDoor()
    {
        AudioSource.PlayClipAtPoint(opening, transform.position, .5f);
        Animation animation = door.GetComponent<Animation>();
        animation.Play();
        _collider.enabled = false;
    }


    public void ApplyGeneratedCode(string code)
    {
        properCombination = code;
        
    }
    public void ShowGeneratedCode()
    {
        if (!isLvl2)
        {
            letter.combination += properCombination;
        }
        else
        {
            textField.text = properCombination;
        }
    }

    public void Unseal()
    {
        RpcOpenDoor();
    }

    public bool Check()
    {
        throw new System.NotImplementedException();
    }
}
