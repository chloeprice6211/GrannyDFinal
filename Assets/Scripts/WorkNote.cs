using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class WorkNote : Device, IReceivePassword, IReadable
{
    [Header("WORK NOTE")]

    [SerializeField] Animation noteAnimation;

    [SerializeField] AnimationClip openNoteAClip;
    [SerializeField] AnimationClip closeNoteAClip;

    [SerializeField] AudioClip bookUseClip;
    [SerializeField] AudioClip bookExitClip;

    [SerializeField] TextMeshProUGUI officePasswordTF;

    [SerializeField] Canvas readUICanvas;

    public UIReaderData readerData;

    private void Start()
    {
        readerData.tableEntry = "inGameItems";
    }

    public override void UseDevice(InputAction.CallbackContext context)
    {
        if (isBeingInspected || inspectAnimation.isPlaying) return;
        if (noteAnimation.isPlaying) return;
        _owner.currentReadableObject = this;
        AudioSource.PlayClipAtPoint(bookUseClip, transform.position);
        noteAnimation.Play(openNoteAClip.name);
        readUICanvas.enabled = true;
        base.UseDevice(context);
    }

    public override void OnDropItem()
    {
        if(_owner != null)
        {
            _owner.currentReadableObject = null;
        }
        
        base.OnDropItem();  
    }

    public override void ExitDevice(InputAction.CallbackContext context)
    {
        if (noteAnimation.isPlaying) return;

        _owner.currentReadableObject = null;
        readUICanvas.enabled = false;
        AudioSource.PlayClipAtPoint(bookExitClip, transform.position);
        noteAnimation.Play(closeNoteAClip.name);
        base.ExitDevice(context);
    }

    public void DisplayPassword(string code)
    {
        officePasswordTF.text = "wifi   " + code;
    }

    public void EnterReadData(NetworkPlayerController owner)
    {
        readerData.passcode = officePasswordTF.text;
        UIManager.Instance.Reader.ShowReaderPanel(readerData, owner);
    }
}
