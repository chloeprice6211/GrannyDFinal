using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using static UnityEngine.UI.GridLayoutGroup;

public class NotesReader : MonoBehaviour
{
    [SerializeField] Animation readerAnimation;
    [SerializeField] GameObject readerPanel;
    [SerializeField] TextMeshProUGUI titleTF;
    [SerializeField] TextMeshProUGUI contentTF;

    public bool isActive;

    NetworkPlayerController _ownerCopy;
    Item _item;

    public void ShowReaderPanel(UIReaderData readerData, NetworkPlayerController owner)
    {
        CancelInvoke();

        titleTF.text = string.Empty;
        contentTF.text = string.Empty;

        readerAnimation["readerHolderIn"].speed = 1;
        readerAnimation.Play("readerHolderIn");

        if (Inventory.Instance.GetMainItemOut(owner, out _item))
        {
            if (_item is Device) (_item as Device).Controls.Disable();
        }

        isActive = true;
        _ownerCopy = owner;
        readerPanel.SetActive(true);

        owner.pauseInput.Disable();
        owner.readerOutInput.Enable();

        titleTF.text = LocalizationSettings.StringDatabase.GetLocalizedString("notes", readerData.titleKey);
        string _content = LocalizationSettings.StringDatabase.GetLocalizedString("notes", readerData.contentKey);

        if (!string.IsNullOrEmpty(readerData.passcode))
        {
            _content = _content.Replace("{passcode}", readerData.passcode);
        }

        MessagePrint.Instance.Message(_content, contentTF, null, 0.015f, true);
    }

    public void CloseReaderPanel()
    {
        float _animCurrentTime;

        if (readerAnimation.isPlaying)
        {
            _animCurrentTime = readerAnimation["readerHolderIn"].time;
            readerAnimation.Stop();
        }
        else
        {
            _animCurrentTime = readerAnimation["readerHolderIn"].length;
        }

        readerAnimation["readerHolderIn"].time = _animCurrentTime;
        readerAnimation["readerHolderIn"].speed = -1;
        readerAnimation.Play("readerHolderIn");

        if (Inventory.Instance.GetMainItemOut(_ownerCopy, out _item) && !_ownerCopy.journal.isActive)
        {
            Device _device = _item is Device? _item as Device : null;
            if (_device != null && _device.isBeingUsed)
            {
                _device.Controls.Enable();
            }
            else
            {
                _ownerCopy.pauseInput.Enable();
            }
        }
        else
        {
            _ownerCopy.pauseInput.Enable();
        }

        isActive = false;
        
        MessagePrint.Instance.StopPrinting();

        _ownerCopy = null;

        Invoke(nameof(DisablePanelDelayed), readerAnimation["readerHolderIn"].length);
    }

    void DisablePanelDelayed()
    {
        readerPanel.SetActive(false);
    }
}
