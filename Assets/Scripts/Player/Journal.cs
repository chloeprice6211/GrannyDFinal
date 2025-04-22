using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Journal : MonoBehaviour
{
    [SerializeField] GameObject content;
    [SerializeField] GameObject notesHolder;

    [SerializeField] AudioClip switchPage;
    [SerializeField] AudioClip getJournal;
    [SerializeField] AudioClip journalAway;

    [SerializeField] NetworkPlayerController playerController;

    private PlayerControls _journalControls;
    private Animation _journalAnimation;
    private GameObject _item;
    private IHasControls _controlsItem;

    private int currentNoteIndex = 0;

    public List<Note> Notes = new();

    public bool isActive;

    public void Awake()
    {
        _journalControls = new();

        _journalControls.Journal.Next.performed += SwitchNextPage;
        _journalControls.Journal.Previous.performed += SwitchPreviousPage;

        _journalAnimation = content.GetComponent<Animation>();
       
    }

    public void ShowOrHideJournal()
    {
        if (!playerController.hasAuthority) return;
        if (isActive && _journalAnimation.isPlaying) return;

        if (isActive)
        {
            AudioSource.PlayClipAtPoint(journalAway, transform.position);

            _journalAnimation.Play("HideJournal");

            playerController.currentReadableObject = null;

            playerController.EnablePlayerControls();

            _journalControls.Journal.Disable();
            Invoke("HideAfterAnimation", _journalAnimation.GetClip("HideJournal").length);

            //if(_controlsItem != null)
            //{
            //    _controlsItem.ReturnControls();
            //    _controlsItem = null;
            //}

            if(playerController.flashlight != null && !playerController.flashlight.isOn)
            {
                playerController.flashlight.SwitchFlashlight(false);
                playerController.flashlight.EnableControls();
            }
        }
        else
        {
            if (Inventory.Instance.HasAnyItem(playerController))
            {
                Item item = Inventory.Instance.GetMainItem(playerController);

                if(item is IHasControls)
                {
                    _controlsItem = item as IHasControls;
                    _controlsItem.DisableItemControls();
                }
                
            }

            if (playerController.flashlight != null && playerController.flashlight.isOn)
            {
                playerController.flashlight.SwitchFlashlight(false);
                playerController.flashlight.DisableControls();
            }

            AudioSource.PlayClipAtPoint(getJournal, transform.position);

            playerController.DisablePlayerControls(false, true, false);

            content.gameObject.SetActive(true);

            _journalAnimation.Play("ShowJournal");

            _journalControls.Journal.Enable();

            if (Notes.Count > 0)
            {
                ShowNote(currentNoteIndex);
                playerController.currentReadableObject = Notes[currentNoteIndex] as IReadable;
            }

            isActive = true;

        }
        
    }
    
    public void HideAfterAnimation()
    {
        isActive = false;
        content.gameObject.SetActive(false);

        //notesHolder.transform.GetChild(currentNoteIndex).gameObject.SetActive(false);
        playerController.EnablePlayerControls();

        if (_controlsItem != null && Inventory.Instance.HasAnyItem(playerController))
        {
            if (Inventory.Instance.GetMainItem(playerController) is not IHasControls) return;

            _controlsItem.ReturnControls();
            _controlsItem = null;
        }


    }

    private void SwitchNextPage(InputAction.CallbackContext callback)
    {
        int oldIndex = currentNoteIndex;
        
        if(Notes.Count < 2){
            return;
        }
        else if (Notes.Count - 1 <= currentNoteIndex)
        {
            currentNoteIndex = 0;
            //return;
        }
        else currentNoteIndex += 1;

        AudioSource.PlayClipAtPoint(switchPage, transform.position);

        playerController.currentReadableObject = Notes[currentNoteIndex];

        notesHolder.transform.GetChild(oldIndex).gameObject.SetActive(false);
        notesHolder.transform.GetChild(currentNoteIndex).gameObject.SetActive(true);

    }

    private void SwitchPreviousPage(InputAction.CallbackContext callback)
    {
        int oldIndex = currentNoteIndex;

        if (currentNoteIndex < 1)
        {
            return;
        }
        else currentNoteIndex -= 1;

        AudioSource.PlayClipAtPoint(switchPage, transform.position);

        playerController.currentReadableObject = Notes[currentNoteIndex];

        notesHolder.transform.GetChild(oldIndex).gameObject.SetActive(false);
        notesHolder.transform.GetChild(currentNoteIndex).gameObject.SetActive(true);

    }

    public void AddNote(Note note)
    {
        note.highlightLight.enabled = false;
        Notes.Add(note);

        note.transform.SetParent(notesHolder.transform);

        note.transform.localPosition = Vector3.zero;
        note.transform.localRotation = Quaternion.identity;

        note.gameObject.SetActive(false);

    }

    public void ShowNote(int index)
    {
        notesHolder.transform.GetChild(index).gameObject.SetActive(true);
    }

    private void OnDestroy()
    {
        _journalControls.Disable();
    }
}
