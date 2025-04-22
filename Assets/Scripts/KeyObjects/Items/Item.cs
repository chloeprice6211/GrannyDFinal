using JetBrains.Annotations;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class Item : Mirror.NetworkBehaviour, ITakeable, IHighlight
{
    [Header("item")]
    [Space]

    [Header("general")]
    protected Collider _collider;
    protected NetworkPlayerController _owner;
    [SerializeField] string oneTimeTip;
    [SerializeField] string oneTimeAudioKey;
    public Sprite iconSprite;
    public ItemList itemName;

    public bool isCompact;
    public Vector3 inventoryCustomRotation;
    public Vector3 invetoryCustomPosition;

    [Space]
    [Header("inspect")]
    public bool canBeInspected;
    public bool isBeingInspected;

    public Animation inspectAnimation;
    [SerializeField] protected AnimationClip enterInspectClip;
    [SerializeField] protected AnimationClip quitInspectClip;
    public string inspectLabel;
    [TextArea]
    public string inspectDescription;

    [Space]
    [Header("misc")]
    public Outline outlineShader;
    public Light highlightLight;
    public AudioClip keyItemAudio;

    static bool s_isOutlineAllowed;
    static bool s_canBeTaken;
    static bool s_hasControlsTipDisplayed;


    bool hasInteracted;

    [SerializeField] protected AudioClip takeItemSound;
    IEnumerator inspectRoutine;

    PlayerControls _inspectControls;
    Vector2 mouseLook;

    float xRotation;

    public Vector3 parentScale;

    public virtual void Awake()
    {
        _collider = GetComponent<Collider>();
        inspectRoutine = InspectRoutine();
        _inspectControls = new();
        parentScale = transform.parent.localScale;
    }

    public virtual void OnDropItem()
    {
        
        //if (inspectAnimation.isPlaying)
        //{
        //    Debug.Log("returining");
        //    ReturnFromInspect();
        //    inspectAnimation.Stop();
        //}
        //if (isBeingInspected)
        //{
        //    Debug.Log("returining");
        //    UIManager.Instance.HideInspectInfo();
        //}



        if(_owner != null)
        {
            _owner.InventoryScript.CurrentSlots++;
        }
        

        _owner = null;




        gameObject.layer = 7;
        AudioSource.PlayClipAtPoint(takeItemSound, transform.position, .2f);
        if (ObjectSpawnerManager.Instance.isOutlineAllowed) HighlightLight();

        //DisableHighlight();

    }

    public virtual void OnItemUsed(NetworkPlayerController owner)
    {
        owner.InventoryScript.ClearItem(owner.InventoryScript.items.IndexOf(this));
    }

    public virtual void TakeItem(NetworkPlayerController owner)
    {
        if (owner.hasAuthority)
        {
            if (!hasInteracted && oneTimeTip != string.Empty)
            {
                if(keyItemAudio != null){
                    UIManager.Instance.PlaySound(keyItemAudio, .5f);
                }
                UIManager.Instance.Message(oneTimeTip, oneTimeAudioKey);
                hasInteracted = true;
            }
            if (!s_hasControlsTipDisplayed)
            {
                UIManager.Instance.controlsMessageTip.ControlsMessage("inspectTip");
                UIManager.Instance.controlsMessageTip.ControlsMessage("itemDropTip");

                s_hasControlsTipDisplayed = true;
            }
        }

        if (ObjectSpawnerManager.Instance.isOutlineAllowed) DisableHighlightLight();

        AudioSource.PlayClipAtPoint(takeItemSound, transform.position, .5f);
        //Inventory.Instance.TakeItemClient(this, owner);
        gameObject.layer = 13;
        _owner = owner;

    }

    public virtual void Inspect()
    {
        if (isBeingInspected) return;

        if(_owner.isDropCommandBeingPerformed)
        {
            return;
        }

        if (_owner == null) {
            return;
        }

        _inspectControls.Enable();
        isBeingInspected = true;

        UIManager.Instance.PrintItemInspectInfo(LocalizationSettings.StringDatabase.GetLocalizedString("itemDescriptionTitles",inspectLabel),
            LocalizationSettings.StringDatabase.GetLocalizedString("itemDescription", inspectDescription));

        if (canBeInspected)
        {
            inspectAnimation.Play(enterInspectClip.name);
            StartCoroutine(inspectRoutine);
        }
    }

    public virtual void ReturnFromInspect()
    {
        return;
    }

    IEnumerator InspectRoutine()
    {
        xRotation = 0f;

        while (true)
        {
            mouseLook = Vector2.zero;
            mouseLook = _inspectControls.Player.Look.ReadValue<Vector2>();

            xRotation -= mouseLook.x / 2;
            transform.parent.localRotation = Quaternion.Euler(inventoryCustomRotation.x - 10, xRotation, 0f);

            yield return null;
        }
    }

    public void Highlight()
    {
        outlineShader.enabled = true;
    }

    public void HighlightLight()
    {
        highlightLight.enabled = true;
    }

    public void DisableHighlightLight()
    {
        highlightLight.enabled = false;
    }

    public void DisableHighlight()
    {
        outlineShader.enabled = false;
    }

    public void ChangeHighlightThickness(float value)
    {
        outlineShader.OutlineWidth = value;
    }
}
public enum ItemList
{
    None,
    l1GarageRemote,
    l1Laptop,
    battery,
    l1usb,
    l2Laptop,
    l2Remote,
    l2OsDisc,
    l3WalkieTalkie,
    lvl4lockpick,
    lvl4Pliers,
    lvl4Fuse,
    lvl4TeddyBear
}
