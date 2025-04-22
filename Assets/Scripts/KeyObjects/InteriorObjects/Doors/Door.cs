using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;

public class Door : Mirror.NetworkBehaviour, IInteractableRpc, IGhostInteractable, IUnlockable, IHighlight
{
    public ObjectiveType objectiveType;

    [SerializeField] Outline outline;

    [SerializeField] List<GameObject> handles;
    [SerializeField] GameObject doorPivot;
    [SerializeField] Animation keyHoleAnimation;
    [SerializeField] Transform keyInstallPosition;

    public AudioClip attemp;
    public AudioClip success;

    public AudioClip opening;
    public AudioClip closing;
    public AudioClip noKeyAttemp;

    [SerializeField] string[] animationName;

    [SerializeField] AnimationClip doorOpening;
    [SerializeField] AnimationClip doorClosing;

    [Range(0, 1)]
    public float doorInteractionSoundVolume = .35f;

    [Range(0, 1)]
    public float doorUnlockSoundVolume = 1f;

    private Animation _doorInteractAnimation;
    private Animation[] _doorHandleAnimation = new Animation[2];

    [SyncVar] public bool isSealed;
    public bool isClosed = true;
    public bool mustChangeTrigger;

    Collider _collider;

    [Header("interact tips")]

    [TextArea]
    public string onLockedInteractTip;
    public string onLockedInteractAudioKey;
    public string onLockedInteractWithKeyAudioKey;

    string onLockedInteractWithKeyTip;
    string onLockedInteractWithItemTip;

    bool _isCharacterMessageInCooldown;

    public DoorScreamer screamer;


    public virtual void Start()
    {
        if (onLockedInteractTip == null || onLockedInteractTip == string.Empty)
            onLockedInteractTip = "useAKey";
        if (onLockedInteractWithKeyTip == null || onLockedInteractWithKeyTip == string.Empty)
            onLockedInteractWithKeyTip = "useAnotherKey";
        if (onLockedInteractWithItemTip == null || onLockedInteractWithItemTip == string.Empty)
            onLockedInteractWithItemTip = "useAKey";

        if (string.IsNullOrEmpty(onLockedInteractAudioKey))
            onLockedInteractAudioKey = "useKey_A";

        if (string.IsNullOrEmpty(onLockedInteractWithKeyAudioKey))
            onLockedInteractWithKeyAudioKey = "useAnotherKey_A";

        if (onLockedInteractTip != "useAKey")
        {
            onLockedInteractWithItemTip = onLockedInteractTip;
            onLockedInteractWithKeyTip = onLockedInteractTip;
        }

        _doorInteractAnimation = doorPivot.GetComponent<Animation>();
        _collider = GetComponent<Collider>();

        if (handles.Count > 0)
        {
            _doorHandleAnimation[0] = handles[0].GetComponent<Animation>();
            _doorHandleAnimation[1] = handles[1].GetComponent<Animation>();
        }
    }


    public virtual void Interact(NetworkPlayerController owner)
    {
        if (isSealed)
        {
            if (Inventory.Instance.HasAnyItem(owner))
            {
                Item key = Inventory.Instance.GetMainItem(owner);

                if(this is Locker || this is LockerDoor)
                {
                    DelayCharacterMessage("lockerLocked", "lockerLocked_A");
                    //UIManager.Instance.Message("lockerLocked", "lockerLocked_A");
                }
                else if (key is KeyCard)
                {
                    //UIManager.Instance.Message("reader", "reader_A");
                    DelayCharacterMessage("reader", "reader_A");
                }
                else if (key is Key)
                {
                    if ((key as Key).objectiveType == objectiveType)
                    {
                        key.OnItemUsed(owner);
                        //UIManager.Instance.Message("Got it.");
                        UnlockDoorCommand(key);
                    }
                    else
                    {
                        //UIManager.Instance.Message(onLockedInteractWithKeyTip, onLockedInteractWithKeyAudioKey);
                        DelayCharacterMessage(onLockedInteractWithKeyTip, onLockedInteractWithKeyAudioKey);
                        AudioSource.PlayClipAtPoint(attemp, transform.position);
                    }
                }
                else
                {
                    DelayCharacterMessage(onLockedInteractTip, onLockedInteractAudioKey);
                   //UIManager.Instance.Message(onLockedInteractTip, onLockedInteractAudioKey);
                    AudioSource.PlayClipAtPoint(noKeyAttemp, transform.position, doorUnlockSoundVolume);
                }
            }
            else
            {
                if (this is LockerDoor)
                {
                    DelayCharacterMessage("lockerLocked", "lockerLocked_A");
                   //UIManager.Instance.Message("lockerLocked", "lockerLocked_A");
                }
                else
                {
                    if (handles.Count > 0)
                    {
                        _doorHandleAnimation[0].Play();
                        _doorHandleAnimation[1].Play();
                    }

                    DelayCharacterMessage(onLockedInteractTip, onLockedInteractAudioKey);
                   //UIManager.Instance.Message(onLockedInteractTip, onLockedInteractAudioKey);
                }

                AudioSource.PlayClipAtPoint(noKeyAttemp, transform.position);

            }
        }
        else if (!isSealed && !_doorInteractAnimation.isPlaying)
        {
            OpenCloseDoorCommand();
        }
    }


    public virtual void OpenDoor()
    {
        if(screamer is not null){
            screamer.OpenGhostDoor();
        }

        isClosed = false;
        tag = "OpenDoor";

        if (handles.Count > 0)
        {
            _doorHandleAnimation[0].Play();
            _doorHandleAnimation[1].Play();
        }

        _doorInteractAnimation.Play(doorOpening.name);
        AudioSource.PlayClipAtPoint(opening, transform.position, doorInteractionSoundVolume);

    }

    public virtual void CloseDoor()
    {
        tag = "ClosedDoor";
        isClosed = true;
        _doorInteractAnimation.Play(doorClosing.name);
        AudioSource.PlayClipAtPoint(closing, transform.position, doorInteractionSoundVolume);

    }

    #region Client/server

    //open close 
    public void OpenCloseDoor()
    {
        if (isClosed)
        {
            OpenDoor();
        }
        else
        {
            CloseDoor();
        }
    }

    [Command(requiresAuthority = false)]
    public void OpenCloseDoorCommand()
    {
        OpenCloseDoorRpc();
    }

    [ClientRpc]
    private void OpenCloseDoorRpc()
    {
        OpenCloseDoor();

        if (mustChangeTrigger)
        {
            StartCoroutine(OpenCloseRoutine());
        }
    }

    //unlocking
    public virtual void UnlockDoor(Item key)
    {
        isSealed = false;

        AudioSource.PlayClipAtPoint(success, transform.position, doorUnlockSoundVolume);

        Transform itemParent = key.transform.parent;

        itemParent.SetParent(keyInstallPosition);
        itemParent.localPosition = Vector3.zero;
        itemParent.localRotation = Quaternion.identity;

        key.GetComponent<Collider>().enabled = false;
        keyHoleAnimation.Play();
    }

    [Command (requiresAuthority = false)]
    public void UnlockDoorCommand(Item key)
    {
        UnlockDoorRpc(key);
    }

    [ClientRpc]
    public void UnlockDoorRpc(Item key)
    {
        UnlockDoor(key);
    }

    #endregion

    [ClientRpc]
    public void PerformGhostInteraction()
    {
        if (isSealed)
        {
            return;
        }

        if (isClosed)
        {
            OpenDoor();
        }
        else
        {
            CloseDoor();
        }
    }

    void DisableCollider()
    {
        DisableColliderCmd();
    }

    [Command(requiresAuthority = false)]
    void DisableColliderCmd()
    {
        DisableColliderRpc();
    }

    [ClientRpc]
    void DisableColliderRpc()
    {
        StartCoroutine(OpenCloseRoutine());
    }

    IEnumerator OpenCloseRoutine()
    {
        _collider.isTrigger = true;

        while (_doorInteractAnimation.isPlaying)
        {
            yield return null;
        }

        _collider.isTrigger = false;
    }

    public void Unseal()
    {
        isSealed = false;
        AudioSource.PlayClipAtPoint(success, transform.position);
    }

    public void Highlight()
    {
        if (outline != null)
            outline.enabled = true;
    }

    public void DisableHighlight()
    {
        if (outline != null)
            outline.enabled = false;
    }

    public void ChangeHighlightThickness(float value)
    {
        if (outline != null)
            outline.OutlineWidth += value;
    }

    void DelayCharacterMessage(string key1, string key2)
    {
        if (_isCharacterMessageInCooldown) return;
        UIManager.Instance.Message(key1, key2);
        StartCoroutine(DelayRoutine());
    }

    IEnumerator DelayRoutine()
    {
        _isCharacterMessageInCooldown = true;

        yield return new WaitForSeconds(10f);

        _isCharacterMessageInCooldown = false;
    }
    [Command (requiresAuthority = false)]
    public void UnlockDoorWOKeyCmd(){
        isSealed = false;
    }

    public bool Check()
    {
        throw new System.NotImplementedException();
    }
}
 