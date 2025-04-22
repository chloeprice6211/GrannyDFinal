using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.PostProcessing;
using Cinemachine;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using Steamworks;
using UnityEngine.Animations.Rigging;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization;
using System;
using System.Linq.Expressions;


public class NetworkPlayerController : Mirror.NetworkBehaviour
{   
    //this is a test
    public static NetworkPlayerController NetworkPlayer;

    [Header("Components")]
    public Camera playerCamera;
    public CinemachineVirtualCamera virtualCamera;
    public Journal journal;
    public PillEffect pillEffect;
    [SerializeField] GameObject journalGameObject;
    [SerializeField] GameObject lightSource;
    public Flashlight flashlight;
    public Transform inventory;
    public Transform flashlightPosition;
    [SerializeField] public Transform chairPosition;
    [SerializeField] Animator animator;
    public GameObject body;
    public ClientsFlashlight clientFlashlight;
    public Inventory InventoryScript;
    public Transform Backpack;

    [Header("Movement")]
    [SerializeField] float moveSpeed = 2f;
    [SerializeField] Transform groundCheckPosition;
    [SerializeField] LayerMask groundMask;
    public float groundDistance = .4f;
    private bool isGrounded;
    private const float gravity = -9.81f;
    public float maxSpeed = 2;
    public float staminaCap = 3;
    public float staminaDelay = 6;
    float _currentStamina;

    //movement 
    private Vector2 _inputVector;
    private Vector3 _moveDirection;
    private Vector3 _velocity;

    //controls
    private PlayerControls _controls;
    public CharacterController controller;
    public PlayerCamera _cameraController;

    [Header("Ray")]
    private Ray _ray;
    private RaycastHit _impactedObject;
    private bool didHit;
    public LayerMask keyObjectlayerMask;
    public bool isInCar;
    Collider _impactedCollider;
    public Transform dropPosition;

    [Header("Sounds")]
    [SerializeField] List<AudioClip> scaredModeSoundClips;
    [SerializeField] AudioSource hearBeatingSource;
    [SerializeField] AudioSource footstepsAudioSource;
    [SerializeField] AudioSource oneShotSource;
    [SerializeField] List<AudioClip> footstepAudioClips;
    [SerializeField] AudioClip deathSound;
    [SerializeField] AudioClip startFearSound;
    [SerializeField] AudioClip preDeathSound;
    [SerializeField] AudioClip startHearbeatingSound;

    [SerializeField] List<string> fearAudioKeys;

    [Header("Skins")]
    [SerializeField] List<Material> tShirtMaterials;
    [SerializeField] List<Material> jeansMaterials;
    [SerializeField] List<Material> eyeMaterials;

    [SerializeField] List<GameObject> hairSkin;
    [SerializeField] Renderer tShirtRenderer;
    [SerializeField] Renderer jeansRenderer;
    [SerializeField] Renderer eyeRenderer;

    [Header("Rigs")]
    public Rig flashlightHandRig;
    public Rig aimRig;

    [Header("misc")]
    public FocusObject _operatedFocusObject;
    [SerializeField] GameObject ghostKillingObject;
    IEnumerator reviveRoutine;
    public Transform pelvis;

    [SerializeField] AudioSource characterVoiceAudioSource;

    public float footstepRate;
    private float _currentFootstepRate;
    float _regulatFootstepRate;

    private bool _isScared;
    float _highlightTimer;
    [HideInInspector]
    public bool hasBeenShocked;
    [SyncVar] public bool isAlive = true;
    public ReviveHitbox reviveHitbox;

    //animator
    private int _velocityXHash;
    private int _velocityZHash;
    private int _hasDiedHash;
    private int _sittingHash;
    private int _movementHash;

    Vector2 _currentInputVector = Vector2.zero;
    Vector2 _currentVelocity;
    Vector3 _localVector = Vector2.zero;
    float _smoothTime = .2f;

    bool _isSprinting;
    bool _canSprint;

    bool _mustPlay;

    bool isTutorial;
    string _currentName = string.Empty;
    public string _currentTag = "Untagged";

    //interface vars
    IHighlight _prevItem;
    ITakeable _iTakeableItem;
    IInteractableRpc _iInteractableObject;

    public GameObject screamerObject;

    public IReadable currentReadableObject;

    //input actions;
    public InputAction pauseInput;
    public InputAction inspectInput;
    public InputAction readerOutInput;
    public InputAction interactInput;
    public InputAction inventoryInput;
    public InputAction dropInput;
    public InputAction switchInput;

    public InputActionMap inventoryActionMap;

    public bool isDropCommandBeingPerformed;
    public bool isSwitchItemCommandBeingPerformed;
    public bool isItemBeingTaken;
    public bool isItemBeingSet;
    public bool isItemBeingReplaced;
    public bool action;
    public bool isBeingTakenToBackpack;

    public bool isRpcBeingProcessed;

      float screamerTime =700f;
     float screamerTimer = 0f;
     float screamerNextTime = 1000f;

    public Animation screamerAnimation;
    public List<AudioClip> screamerClips;
    bool _isBusy;
    public bool isBusy{
        get{return _isBusy;}
        set{
            _isBusy = value;

            if(!_isBusy && screamerReady){
                Screamer();
            }
        }
    }
    public bool screamerReady;
    #region network events

    private void Awake()
    {
        if (SceneManager.GetActiveScene().name == "TutorialScene") isTutorial = true;
    }

    public override void OnStartAuthority()
    {
        NetworkPlayer = this;

        if (!isTutorial)
        {
            GhostEvent.Instance.OnHuntStart.AddListener(OnStartHunt);
            GhostEvent.Instance.OnHuntEnd.AddListener(OnEndHunt);

            GhostEvent.Instance.OnRageStart.AddListener(OnRageStart);
            GhostEvent.Instance.OnRageEnd.AddListener(OnRageEnd);

            ObjectSpawnerManager.Instance.ShowCodes();
            ObjectSpawnerManager.Instance.playersCount++;
        }

        base.OnStartAuthority();

        reviveRoutine = ReviveRoutine();

        _controls = new();
        _controls.Disable();
        journal = journalGameObject.GetComponent<Journal>();
        controller = GetComponent<CharacterController>();

        _controls.Player.Interact.started += InteractPerformed;
        _controls.Player.AlternativeInteract.performed += AlternativeInteractPerformed;
        _controls.Player.OpenJournal.performed += OpenJournalPerformed;
        _controls.Player.DropItem.started += DropItemPerformed;
        _controls.Player.Crouch.performed += CrouchPerformed;
        _controls.UI.Pause.started += PausePerformed;
        //_controls.Player.Inspect.started += InspectPerformed;

        _controls.Inventory.InventoryInOut.started += InventoryInOutStarted;
        _controls.Inventory.GetFirstInventorySlot.performed += GetInventorySlotStarted;

        //_controls.Player.Inventory.started += InventoryStarted;

        _controls.UI.ReaderIn.started += ReaderInStarted;
        _controls.UI.ReaderOut.started += ReaderOutStarted;

        _controls.Player.Sprint.performed += SprintPerformed;
        _controls.Player.Sprint.canceled += SprintCancelled;

        _controls.Player.Enable();
        _controls.UI.Enable();
        _controls.Player.Interact.Disable();
        _controls.Inventory.Enable();

        body.transform.localScale = Vector3.zero;

        pauseInput = _controls.UI.Pause;
        inspectInput = _controls.Player.Inspect;
        readerOutInput = _controls.UI.ReaderOut;
        interactInput = _controls.Player.Interact;
        inventoryInput = _controls.UI.InventoryInOut;
        dropInput = _controls.Player.DropItem;
        switchInput = _controls.Inventory.GetFirstInventorySlot;

        inventoryActionMap = _controls.Inventory;

        

        //disable inputs
        //_controls.UI.ReaderIn.Disable();
        readerOutInput.Disable();

        StartCoroutine(StartingRoutine());
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        CustomNetworkManager.Instance.GamePlayers.Add(this);
    }
    public override void OnStopClient()
    {
        if (hasAuthority)
        {
            _controls.Disable();
            base.OnStopClient();
            SceneManager.LoadScene("Menu");
            _controls.Player.Disable();
        }
    }

    public override void OnStopLocalPlayer()
    {
        base.OnStopLocalPlayer();
    }

    public override void OnStopAuthority()
    {
        _controls.Disable();

        if (Inventory.Instance.HasAnyItem(this))
        {
            Inventory.Instance.DropItemOnTheGround(this);
        }

        base.OnStopAuthority();
    }

    #endregion

    IEnumerator StartingRoutine()
    {
        while (CustomNetworkManager.Instance.LobbyPlayers.Count != CustomNetworkManager.Instance.GamePlayers.Count)
        {
            yield return null;
        }

        if (!isTutorial)
        {
            yield return new WaitForSeconds(1f);
            oneShotSource.PlayOneShot(startHearbeatingSound);
            yield return new WaitForSeconds(1f);

            if (ObjectSpawnerManager.Instance.playersCount > 1)
            {
                if (isServer)
                {
                    UIManager.Instance.Message(GameManager.Instance.lvlHostStartPhrase, "entry-a");
                    UIManager.Instance.Message(GameManager.Instance.lvlHostSecondPhrase, "entryAddCoop-a");
                }
                else
                {
                    UIManager.Instance.Message(GameManager.Instance.lvlClientStartPhrase, "entry-a");
                    UIManager.Instance.Message(GameManager.Instance.lvlClientSecondPhrase, "entryAddCoop-a");
                }
            }
            else
            {
                UIManager.Instance.Message(GameManager.Instance.lvlClientStartPhrase, "entry-a");
                UIManager.Instance.Message(GameManager.Instance.lvlClientSecondPhrase, "entryAddSolo-a");
            }


        }

        yield return new WaitForSeconds(4f);
        _controls.Player.Interact.Enable();
        UIManager.Instance.crosshairAnimation.Play();

        ChangeSkins();

    }

    #region unity callbacks

    private void Start()
    {
        isDropCommandBeingPerformed = false;

        _cameraController = GetComponent<PlayerCamera>();
        _currentFootstepRate = footstepRate;

        _velocityXHash = Animator.StringToHash("VelocityX");
        _velocityZHash = Animator.StringToHash("VelocityZ");
        _hasDiedHash = Animator.StringToHash("hasDied");
        _sittingHash = Animator.StringToHash("Sitting");
        _movementHash = Animator.StringToHash("Movement Blend Tree");
    }

    private void Update()
    {
        if (!hasAuthority) return;

         #region screamer

        screamerTimer+=Time.deltaTime;
        if(screamerTimer >= screamerTime){
            if(isAlive && isBusy){
                screamerReady = true;
            }
            else if(isAlive && !isBusy){
                Screamer();
                ResetScreamer();
            }
           
        }

        #endregion

        #region interaction

        _ray = playerCamera.ViewportPointToRay(new Vector3(.5f, .5f, 0));
        didHit = Physics.Raycast(_ray, out _impactedObject, PlayerAttributes.interactRange, keyObjectlayerMask);

        if (didHit && _controls.Player.Interact.enabled)
        {
            _impactedCollider = _impactedObject.collider;
            if (_impactedCollider.name != _currentName || !_impactedCollider.CompareTag(_currentTag))
            {
                if (!ObjectSpawnerManager.Instance.isOutlineAllowed)
                {
                    if (_prevItem != null)
                        _prevItem.DisableHighlight();

                    if (_impactedObject.collider.TryGetComponent(out _prevItem))
                        _prevItem.Highlight();
                }
                else
                {
                    if (_prevItem != null)
                    {
                        _prevItem.DisableHighlight();
                        _prevItem = null;
                    }

                    if (_impactedObject.collider.TryGetComponent(out _prevItem))
                        _prevItem.Highlight();
                }

                if (!_impactedCollider.CompareTag("Void"))
                {
                    UIManager.Instance.ShowInteractOption(_impactedCollider.tag, 0);
                    _mustPlay = true;
                }
            }

            _currentName = _impactedCollider.name;
            _currentTag = _impactedCollider.tag;

            if (_impactedCollider.CompareTag("Void") && _mustPlay)
            {
                UIManager.Instance.HideInteractOption();
                _mustPlay = false;
            }


        }
        else if (!didHit)
        {
            if (_prevItem != null)
            {
                if (ObjectSpawnerManager.Instance.isOutlineAllowed)
                {
                    if (_prevItem is Item)
                    {
                        _prevItem.DisableHighlight();
                    }
                    else
                    {
                        _prevItem.DisableHighlight();
                    }
                }
                else
                {
                    _prevItem.DisableHighlight();
                }

                _prevItem = null;
            }

            _currentName = "Untagged";
            _currentTag = "Untagged";

            if (_mustPlay)
            {
                UIManager.Instance.HideInteractOption();
                _mustPlay = false;
            }

        }

        #endregion

        #region movement

        isGrounded = Physics.CheckSphere(groundCheckPosition.position, groundDistance, groundMask);

        if (isGrounded && _velocity.y < 0)
        {
            _velocity.y = -2f;
        }

        _inputVector = Vector2.zero;
        _inputVector = _controls.Player.Move.ReadValue<Vector2>();

        Vector3 _targetMoveSpeed = _isSprinting ? _inputVector *= 1.8f : _inputVector;

        _currentInputVector = Vector2.SmoothDamp(_currentInputVector, _targetMoveSpeed, ref _currentVelocity, _smoothTime);


        _moveDirection = transform.right * _currentInputVector.x + transform.forward * _currentInputVector.y;
        _velocity.y += gravity * Time.deltaTime;

        if (controller.enabled)
        {
            controller.Move(_moveDirection * moveSpeed * Time.deltaTime);
            controller.Move(_velocity * Time.deltaTime);
        }

        if (_inputVector != Vector2.zero)
        {
            HandleFootsteps();

            if (_isSprinting && _canSprint)
            {
                _localVector.x = Mathf.Lerp(_localVector.x, _inputVector.x * 2, Time.deltaTime * 2);
                _localVector.y = Mathf.Lerp(_localVector.y, _inputVector.y * 2, Time.deltaTime * 2);
            }
            else if (!_isSprinting)
            {
                _localVector.x = Mathf.Lerp(_localVector.x, _inputVector.x, Time.deltaTime * 8);
                _localVector.y = Mathf.Lerp(_localVector.y, _inputVector.y, Time.deltaTime * 8);
            }
        }
        else
        {
            _localVector.x = Mathf.Lerp(_localVector.x, 0, Time.deltaTime * 6);
            _localVector.y = Mathf.Lerp(_localVector.y, 0, Time.deltaTime * 6);
        }

        HandleStamina();

        animator.SetFloat(_velocityXHash, _localVector.x);
        animator.SetFloat(_velocityZHash, _localVector.y);

        #endregion

       
 }

    #endregion

    #region Interactions with items Client/Server

    [Command]
    public void TakeItemCommand(Item item, NetworkPlayerController owner)
    {
        TakeItemClientRpc(item, owner);
    }

    [Command]
    public void AlternativeInteractWithItemCommand(GameObject item)
    {
        AlternativeInteractWithItemClientRpc(item);
    }

    [ClientRpc]
    public void TakeItemClientRpc(Item item, NetworkPlayerController owner)
    {
        item.TakeItem(this);

        if (owner.hasAuthority)
        {
            owner.isItemBeingSet = false;
        }
    }

    [ClientRpc]
    public void AlternativeInteractWithItemClientRpc(GameObject item)
    {
        item.GetComponent<IAlternativeInteractable>().AlternativeInteract(this);
    }

    public void InteractWithItemRpc(IInteractableRpc item)
    {
        item.Interact(this);
    }

    [ServerCallback]
    void ChangeSkins()
    {
        ChangeSkinRpc();
    }

    [ClientRpc]
    void ChangeSkinRpc()
    {
        Material[] shirtMat = new Material[3];
        Material[] bodyMats = new Material[eyeRenderer.materials.Length];
        Material[] jeansMats = new Material[jeansRenderer.materials.Length];

        for (int a = 0; a < CustomNetworkManager.Instance.GamePlayers.Count; a++)
        {
            for (int s = 0; s < shirtMat.Length; s++)
            {
                shirtMat[s] = tShirtMaterials[a];
            }
            for (int d = 0; d < jeansMats.Length; d++)
            {
                jeansMats[d] = jeansMaterials[a];
            }

            bodyMats = eyeRenderer.materials;
            bodyMats[13] = eyeMaterials[a];

            CustomNetworkManager.Instance.GamePlayers[a].tShirtRenderer.materials = shirtMat;
            CustomNetworkManager.Instance.GamePlayers[a].jeansRenderer.materials = jeansMats;
            CustomNetworkManager.Instance.GamePlayers[a].hairSkin[a].SetActive(true);
            CustomNetworkManager.Instance.GamePlayers[a].eyeRenderer.materials = bodyMats;

        }
    }

    #endregion

    #region input system

    private void InventoryInOutStarted(InputAction.CallbackContext obj)
    {
        UIManager.Instance.uiInventory.ShowOrHide(this);
    }

    private void GetInventorySlotStarted(InputAction.CallbackContext obj)
    {
        if (!hasAuthority) return;
        if (isSwitchItemCommandBeingPerformed) return;
        if (isItemBeingTaken) return;
        if (isItemBeingSet) return;
        if (isItemBeingReplaced) return;
        if (isBeingTakenToBackpack) return;
        if (!action)
        {
            StartCoroutine(Action());
        }
        else
        {
            return;
        }
        isSwitchItemCommandBeingPerformed = true;
        InventoryScript.SwitchItem(Convert.ToInt32(obj.control.name));
    }

    private void ReaderInStarted(InputAction.CallbackContext obj)
    {
        if (!hasAuthority) return;

        if (currentReadableObject != null && !UIManager.Instance.Reader.isActive)
            currentReadableObject.EnterReadData(this);
    }

    private void ReaderOutStarted(InputAction.CallbackContext obj)
    {
        if (!hasAuthority) return;

        if (UIManager.Instance.Reader.isActive)
            UIManager.Instance.Reader.CloseReaderPanel();

    }

    private void SwitchFlashlightPerformed(InputAction.CallbackContext obj)
    {
        flashlight.SwitchFlashlight();
    }

    private void CrouchPerformed(InputAction.CallbackContext obj)
    {
        Screamer();
    }
    IEnumerator Action()
    {
        action = true;
        float a = 0;

        while (a < .2f)
        {

            a += Time.deltaTime;
            yield return null;
        }

        action = false;
        yield return null;
    }

    void InspectPerformed(InputAction.CallbackContext context)
    {
        if (!hasAuthority) return;

        if (isDropCommandBeingPerformed) return;

        if (Inventory.Instance.HasAnyItem(this))
        {
            Item item = Inventory.Instance.GetMainItem(this);

            if (item.inspectAnimation.isPlaying) return;

            if (!item.isBeingInspected)
            {
                DisablePlayerControls(true, true, true, true, true);
                _cameraController.CenterCamera();
                item.Inspect();
            }
            else
            {
                item.ReturnFromInspect();
                EnablePlayerControls();
            }
        }
    }

    void SprintPerformed(InputAction.CallbackContext obj)
    {
        if (!hasAuthority) return;

        _currentFootstepRate = footstepRate / 3;
        _isSprinting = true;
    }

    void SprintCancelled(InputAction.CallbackContext obj)
    {
        if (!hasAuthority) return;

        _isSprinting = false;
        _currentFootstepRate = footstepRate;
    }

    private void PausePerformed(InputAction.CallbackContext obj)
    {
        if (!hasAuthority) return;

        UIManager.Instance.PauseOrUnpauseGame();
    }

    void DropItemPerformed(InputAction.CallbackContext obj)
    {
        if (!hasAuthority) return;

        if (Inventory.Instance.HasAnyItem(this))
        {
            Item _droppedItem = Inventory.Instance.GetMainItem(this);

            if (_droppedItem is Device && (_droppedItem as Device).deviceInteractAnimation.isPlaying)
                return;
            if (_droppedItem.isBeingInspected || _droppedItem.inspectAnimation.isPlaying)
            {
                Debug.Log("it is being inspected");

                return;
            }
            else
            {
                Debug.Log("or not");
            }

            if (ObjectSpawnerManager.Instance.isOutlineAllowed)
            {
                //_droppedItem.Highlight();
                //_droppedItem.ChangeHighlightThickness(1.5f);
            }

            isDropCommandBeingPerformed = true;
            //CmdDropItemOnTheGround(this);
            InventoryScript.DropItemOnTheGround(this);
        }
    }

    private void InteractPerformed(InputAction.CallbackContext callback)
    {
        if (!hasAuthority) return;
        if (isItemBeingTaken) return;
        if (isItemBeingSet) return;
        if (isSwitchItemCommandBeingPerformed) return;
        if (isItemBeingReplaced) return;
        if (isBeingTakenToBackpack) return;
        if (action) return;

        _ray = playerCamera.ViewportPointToRay(new Vector3(.5f, .5f, 0));

        if (Physics.Raycast(_ray, out _impactedObject, PlayerAttributes.interactRange, keyObjectlayerMask))
        {
            if (_impactedObject.collider.TryGetComponent(out _iTakeableItem))
            {

                _prevItem = null;
                (_iTakeableItem as Item).DisableHighlight();

                if (InventoryScript.TakeItemClient(_iTakeableItem as Item, this))
                {
                    StartCoroutine(Action());
                    isItemBeingTaken = true;
                    isItemBeingSet = true;
                    TakeItemCommand(_iTakeableItem as Item, this);
                }

            }
            else if (_impactedObject.collider.TryGetComponent(out _iInteractableObject))
            {
                InteractWithItemRpc(_iInteractableObject);
            }
            else if (_impactedObject.collider.GetComponent<IAlternativeInteractable>() != null)
            {
                IAlternativeInteractable obj = _impactedObject.collider.GetComponent<IAlternativeInteractable>();
                obj.AlternativeInteract(this);
            }
        }

    }

    private void AlternativeInteractPerformed(InputAction.CallbackContext callback)
    {
        if (!hasAuthority) return;
        _ray = playerCamera.ViewportPointToRay(new Vector3(.5f, .5f, 0));

        if (Physics.Raycast(_ray, out _impactedObject, PlayerAttributes.interactRange, keyObjectlayerMask))
        {
            if (_impactedObject.collider.GetComponent<IAlternativeInteractable>() != null)
            {
                AlternativeInteractWithItemCommand(_impactedObject.collider.gameObject);
            }
        }
    }

    private void OpenJournalPerformed(InputAction.CallbackContext callback)
    {
        journal.ShowOrHideJournal();
    }


    public void DisablePlayerControls(bool disableCamera = false, bool disableInteraction = false, bool disableMovement = true, bool disableJournal = false, bool disableDrop = true)
    {
        if (disableMovement)
        {
            _controls.Player.Move.Disable();
        }
        if (disableCamera)
        {
            _cameraController.canMove = false;
        }
        if (disableInteraction)
        {
            _controls.Player.Interact.Disable();
        }
        if (disableJournal)
        {
            _controls.Player.OpenJournal.Disable();
        }
        if (disableDrop)
        {
            _controls.Player.DropItem.Disable();
        }
    }

    public void EnablePlayerControls()
    {
        _controls.Player.Enable();
        _cameraController.canMove = true;
    }

    #endregion

    #region event handlers

    public void OnStartHunt()
    {
        if (hasAuthority)
        {
            EnterScaredMode(TimeRange.Long, ScaredModeProperty.FearMode);
            Invoke(nameof(DelayedOnHunt), 1f);

        }
    }

    void DelayedOnHunt()
    {
        if (UnityEngine.Random.Range(0, 100) < 35)
        {
            int randomIndex = UnityEngine.Random.Range(1, 5);
            UIManager.Instance.Message($"fearLineVar{randomIndex}", $"fearVar{randomIndex}_A");
        }
    }

    public void OnEndHunt()
    {

    }

    public void OnRageStart(){
        Debug.Log("rage is starting for everyone");
        if(!isAlive) return;
        _cameraController.EnableRageEffect();

    }
    public void OnRageEnd(){
        Debug.Log("rage is ending for everyone");
        _cameraController.DisableRageEffect();
    }
    #endregion

    #region functionality

    /// <summary>
    /// Use <c>TimeRange</c> static variables
    /// </summary>
    /// <param name="time">scared mode duration</param>
    /// 

    void HandleStamina()
    {
        if (_isSprinting)
        {
            _currentStamina -= Time.deltaTime;
        }
        else if (_currentStamina <= staminaCap)
        {
            _currentStamina += Time.deltaTime / 1.75f;

            if (_currentStamina >= staminaCap)
            {
                _canSprint = true;
            }
        }

        if (_currentStamina <= 0)
        {
            _isSprinting = false;
            _canSprint = false;
        }
    }

    #region screamer
    public void Screamer(){
        screamerObject.SetActive(true);
        StartCoroutine(ScreamerRoutine());
        ResetScreamer();
    }

    public void ResetScreamer(){
         screamerTimer = 0f;
            screamerTime = screamerNextTime;
            screamerReady = false;
    }

    public void Scream(){
        Debug.Log("DEVICE WAS OFF");
        Screamer();
    }

    IEnumerator ScreamerRoutine(){

        AudioSource.PlayClipAtPoint(screamerClips[UnityEngine.Random.Range(0,screamerClips.Count)], transform.position);
        screamerAnimation.Play("ScreamerUp");
        _cameraController.ShakeCamera(.2f, 1.5f, 1f);
        EnterScaredMode(TimeRange.Short, ScaredModeProperty.ShockMode);
        yield return new WaitForSeconds(.55f);
        
        screamerAnimation.Play("ScreamerForward");
        
        
        yield return new WaitForSeconds(.5f);
        screamerObject.SetActive(false);
        yield return null;
    }
    #endregion
    public void EnterScaredMode(float time, ScaredModeProperty modeProperty)
    {
        if (!hasAuthority) return;

        if (pillEffect.gameObject.activeInHierarchy) return;

        _cameraController.ShakeCamera(time, modeProperty._intensity, modeProperty._rate);

        _isScared = true;

        hearBeatingSource.Play();

        StartCoroutine(_cameraController.EnableScaredScreenEffect(modeProperty));
        Invoke("DisableScaredMode", time);

    }

    private void DisableScaredMode()
    {
        StartCoroutine(_cameraController.DisableScaredScreenEffect());

        _isScared = false;

        hearBeatingSource.Stop();
    }

    public void EnablePillEffect()
    {
        if (pillEffect.gameObject.activeInHierarchy)
        {
            pillEffect.gameObject.SetActive(false);
            pillEffect.gameObject.SetActive(true);
        }
        else
        {
            pillEffect.gameObject.SetActive(true);
        }

        if (_isScared)
        {
            DisableScaredMode();
            _cameraController.EndCameraShaking(.05f);

            SteamUserStats.SetAchievement("PILLS_USE");
            SteamUserStats.StoreStats();
        }
    }

    public void DisableDeathScreen()
    {

    }

    public void Say(string audioKey)
    {

        characterVoiceAudioSource.PlayOneShot(
            LocalizationSettings.AssetDatabase.GetLocalizedAsset<AudioClip>(
                "characterLines", audioKey, LocalizationSettings.AvailableLocales.GetLocale("en")
                )
            );

    }


    [Command]
    void CmdDropItemOnTheGround(NetworkPlayerController onwer)
    {
        RpcDropItemOnTheGround(onwer);
    }
    [ClientRpc]
    void RpcDropItemOnTheGround(NetworkPlayerController owner)
    {
        Inventory.Instance.DropItemOnTheGround(owner);
    }


    private void HandleFootsteps()
    {
        _currentFootstepRate -= Time.deltaTime;

        if (_currentFootstepRate <= 0)
        {
            footstepsAudioSource.PlayOneShot(footstepAudioClips[UnityEngine.Random.Range(0, footstepAudioClips.Count)]);
            _currentFootstepRate = footstepRate;
        }
    }


    public void Sit()
    {
        transform.localRotation = Quaternion.Euler(Vector3.zero);
        animator.Play(_sittingHash);
        CmdMoveBody(this, new Vector3(0, -0.45f, 0.25f), new Vector3(-19f, 0, 0));
        CmdDisableRigs(this);
    }

    [Command]
    void CmdMoveBody(NetworkPlayerController controller, Vector3 pos, Vector3 rot)
    {
        RpcMoveBody(controller, pos, rot);
    }
    [ClientRpc]
    void RpcMoveBody(NetworkPlayerController controller, Vector3 pos, Vector3 rot)
    {
        controller.body.transform.localPosition = pos;
        controller.body.transform.localRotation = Quaternion.Euler(rot);
    }

    public void ExitSitting()
    {
        animator.Play(_movementHash);
        CmdMoveBody(this, new Vector3(0f, -0.896f, 0f), Vector3.zero);
        CmdEnableRigs(this);
    }

    #endregion

    #region rigs

    public void EnableFlashlightRig()
    {
        flashlightHandRig.weight = 1;
        clientFlashlight.gameObject.SetActive(true);
    }

    [Command]
    public void CmdDisableRigs(NetworkPlayerController owner)
    {
        RpcDisableRigs(owner);
    }

    [ClientRpc(includeOwner = false)]
    void RpcDisableRigs(NetworkPlayerController owner)
    {
        owner.aimRig.weight = 0;
        owner.flashlightHandRig.weight = 0;
    }

    [Command]
    public void CmdEnableRigs(NetworkPlayerController owner)
    {
        RpcEnableRigs(owner);
    }

    [ClientRpc(includeOwner = false)]
    void RpcEnableRigs(NetworkPlayerController owner)
    {
        owner.flashlightHandRig.weight = 1;
        owner.aimRig.weight = 1;
    }

    #endregion

    #region death

    public void Die()
    {
        if (_operatedFocusObject != null)
        {
            _operatedFocusObject.Release();
        }
        if (UIManager.Instance.uiInventory.IsActive)
            UIManager.Instance.uiInventory.Close();

        if (UIManager.Instance.Reader.isActive)
            UIManager.Instance.Reader.CloseReaderPanel();

        GameManager.Instance.CmdReduceAlivePlayerCount();
        DisablePlayerControls(true, true);
        isAlive = false;

        _cameraController.virtualCamera.transform.localRotation = Quaternion.Euler(Vector3.zero);

        aimRig.weight = 0;

        CmdDisableRigs(this);

        hearBeatingSource.Stop();

        StopCoroutine(reviveRoutine);

        StartCoroutine(DeathCoroutine());

        //animator.SetBool(_hasDiedHash, true);
        animator.Play("Death");
        //animator.ResetTrigger("death");
        //animator.SetTrigger("death");

        _controls.Player.Disable();

        InventoryScript.DropOnDeath(this);

    }

    [Command]
    void CmdRemoveCollider(NetworkPlayerController controller)
    {
        isAlive = false;
        RpcRemoveCollider(controller);
    }
    [ClientRpc]
    void RpcRemoveCollider(NetworkPlayerController controller)
    {
        controller.controller.enabled = false;
        controller.gameObject.layer = 0;
        reviveHitbox._collider.enabled = true;
    }

    public void Revive(NetworkPlayerController controller)
    {
        controller.controller.enabled = true;
        controller.gameObject.layer = 11;
        controller.reviveHitbox._collider.enabled = false;
        controller.isAlive = true;

        if (!controller.hasAuthority) return;
        GameManager.Instance.CmdIncreaseAlivePlayerCount();

        //EnablePlayerControls();
        Debug.Log("player revived!");
        animator.SetBool("hasDied", false);
        //animator.SetBool("hasRevived", true);
        animator.Play("Revived");

        _cameraController.virtualCamera.transform.Rotate(0, 180, 0);
        _cameraController.virtualCamera.transform.localRotation = Quaternion.Euler(0, _cameraController.virtualCamera.transform.localRotation.y, 0);

        StartCoroutine(ReviveRoutine());

    }

    IEnumerator ReviveRoutine()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        _cameraController.ReturnNormalVisionAfterDeath();

        UIManager.Instance.endingPanel.ShowMainCamera();

        if (UIManager.Instance.endingPanel.gameObject.activeInHierarchy)
        {
            UIManager.Instance.endingPanel.gameObject.SetActive(false);
        }

        yield return new WaitForSeconds(4f);
        EnablePlayerControls();
        CmdEnableRigs(this);
        UIManager.Instance.ShowCrosshair();

    }

    public void SwitchCamera()
    {
        NetworkPlayerController buddy = CustomNetworkManager.Instance.GamePlayers[0].hasAuthority ?
            CustomNetworkManager.Instance.GamePlayers[1] : CustomNetworkManager.Instance.GamePlayers[0];

        _cameraController.virtualCamera.gameObject.SetActive(false);
        buddy._cameraController.virtualCamera.gameObject.SetActive(true);
        
    }

    private IEnumerator DeathCoroutine()
    {
        float counter = 0;
        float rate = 11;

        ghostKillingObject.SetActive(true);
        _cameraController.EnablePreDeathScreenEffect();

        AudioSource.PlayClipAtPoint(preDeathSound, transform.position);
        yield return new WaitForSeconds(4.8f);
        AudioSource.PlayClipAtPoint(deathSound, transform.position, .6f);
        _cameraController.EnableDeathScreenEffect();

        while (counter <= 180)
        {
            counter += rate;
            _cameraController.virtualCamera.transform.Rotate(0, rate, 0);

            yield return null;
        }

        yield return new WaitForSeconds(1f);
        ghostKillingObject.SetActive(false);

        counter = 0;
        rate = .6f;

        while (counter <= 90)
        {
            counter += rate;
            _cameraController.virtualCamera.transform.Rotate(rate, 0, 0);

            yield return null;
        }

        CmdRemoveCollider(this);

        GameManager.Instance.GameOver(Ending.Death);

    }

    public void RemoveDeathEffect()
    {
        _cameraController.DisableDeathScreenEffect();
    }

    #endregion 

    private void OnDisable()
    {
        //_controls.Player.Disable();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "GhostAura")
        {
            if (!hasBeenShocked)
            {
                EnterScaredMode(TimeRange.Short, ScaredModeProperty.ShockMode);
            }
        }
    }

    [QFSW.QC.Command ("tp")]
    void tp(string test){
        controller.enabled = false;
        if(test == "back"){
            
        }
        switch(test){
            case "back":
            transform.position = new Vector3(43.4770012f,0.82099998f,26.7609997f);
            break;
            case "light":
              transform.position = new  Vector3(33.5359993f,-2.11599994f,-18.8579998f);
              break;
              case "ph":
              transform.position = new  Vector3(31.4689999f,0.397000015f,20.9914341f);
              break;
        }
        controller.enabled = true;
        Debug.Log(test);

    }
}

public struct TimeRange
{
    public static float Long = 22;
    public static float Short = 10;
}

public enum ScaredMode
{
    Shock,
    Fear
}

public class ScaredModeProperty
{
    public float _intensity;
    public float _rate;
    public ScaredMode _mode;

    private static readonly ScaredModeProperty fearProperty = new(.25f, .1f, ScaredMode.Fear);
    private static readonly ScaredModeProperty shockedProperty = new(.8f, .1f, ScaredMode.Shock);

    public static ScaredModeProperty FearMode
    {
        get
        {
            return fearProperty;
        }
    }
    public static ScaredModeProperty ShockMode
    {
        get
        {
            return shockedProperty;
        }
    }

    public ScaredModeProperty(float intensity, float rate, ScaredMode mode)
    {
        _intensity = intensity;
        _rate = rate;
        _mode = mode;
    }


}