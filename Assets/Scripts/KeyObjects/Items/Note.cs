using Mirror;
using Mirror.FizzySteam;
using Steamworks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.UI.GridLayoutGroup;

public class Note : NetworkBehaviour, IInteractableRpc, IReadable
{
    PlayerControls noteControls;

    [Header("READER PROPERTIES")]
    public string noteContentKey;
    public string noteTitleKey;
    public int insertIndex;
    public string codeToInsert;

    [SerializeField] AudioClip pageCollect;
    [SerializeField] Canvas noteControlsCanvas;
    public Outline outline;
    public Light highlightLight;

    [HideInInspector]
    public UIReaderData readerData = new();

    NetworkPlayerController ownerCopy;

    IHasControls _itemWithControls = null;

    [TextArea]
    public string message;
    public string audioClipKey;
    public static int s_collectedCount = 0;

    static bool _hasBeenCollected;

    private void Awake()
    {
        readerData.titleKey = noteTitleKey;
        readerData.contentKey = noteContentKey;
        readerData.tableEntry = "notes";

        noteControls = new PlayerControls();
        noteControls.Note.Collect.performed += CollectNote;
    }

    public void Interact(NetworkPlayerController owner)
    {
        if (!owner.hasAuthority) return;

        noteControlsCanvas.enabled = true;
        ownerCopy = owner;

        noteControls.Note.Collect.Enable();

        if (Inventory.Instance.HasAnyItem(owner))
        {
            if(Inventory.Instance.GetMainItem(owner) is IHasControls)
            {
                _itemWithControls = Inventory.Instance.GetMainItem(owner) as IHasControls;
                _itemWithControls.DisableItemControls();
            }
        }

        owner.inventoryInput.Disable();

        if (owner.flashlight!= null && owner.flashlight.isOn)
        {
            owner.flashlight.SwitchFlashlight(false);
        }

        owner.currentReadableObject = this;

        Camera ownerCamera = owner.playerCamera;

        transform.SetParent(ownerCamera.transform);

        transform.localRotation = Quaternion.Euler(0, 0, 0);
        transform.localPosition = new Vector3(0, 0, .27f);

        highlightLight.enabled = false;

        TakeCmd();

        UIManager.Instance.HideInteractOption();
        UIManager.Instance.CanDisplayInteraction = false;
        UIManager.Instance.HideCrosshair();

    }

    [Command (requiresAuthority = false)]
    void TakeCmd()
    {
        TakeRpc();
    }
    [ClientRpc]
    void TakeRpc()
    {
        GetComponent<Collider>().enabled = false;
    }

    public virtual void CollectNote(InputAction.CallbackContext context)
    {
        if (!ownerCopy.hasAuthority) return;
        noteControlsCanvas.enabled = false;
        if (!_hasBeenCollected)
        {
            UIManager.Instance.controlsMessageTip.ControlsMessage("journalTip");
            _hasBeenCollected = true;
        }

        if(_itemWithControls != null)
        {
            _itemWithControls.ReturnControls();
            _itemWithControls = null;
        }

        if (ownerCopy.flashlight!= null && !ownerCopy.flashlight.isOn)
        {
            ownerCopy.flashlight.SwitchFlashlight(false);
        }

        ownerCopy.currentReadableObject = null;

        //ownerJournal.AddNote(gameObject);
       
        if(message != string.Empty)
            UIManager.Instance.Message(message, audioClipKey);

        CollectCmd(this);
        UIManager.Instance.ShowCrosshair();

        AudioSource.PlayClipAtPoint(pageCollect, transform.position, .3f);

        noteControls.Note.Collect.Disable();
        ownerCopy.inventoryInput.Enable();
    }

    [Command (requiresAuthority = false)]
    void CollectCmd(Note note)
    {
        CollectRpc(note);
    }
    [ClientRpc]
    void CollectRpc(Note note)
    {
        s_collectedCount++;
        NetworkPlayerController.NetworkPlayer.journal.AddNote(this);
        note.highlightLight.enabled = false;

        if(s_collectedCount == 9 && SteamManager.Initialized)
        {
            SteamUserStats.SetAchievement("COLLECT_NOTES");
            SteamUserStats.StoreStats();
        }
    }

    private void OnDisable()
    {
        noteControls.Disable();
    }

    public virtual void EnterReadData(NetworkPlayerController owner)
    {
        UIManager.Instance.Reader.ShowReaderPanel(readerData, owner);
    }
}
