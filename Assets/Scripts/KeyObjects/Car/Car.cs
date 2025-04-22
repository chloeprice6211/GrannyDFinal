using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;
using TMPro;

public class Car : Mirror.NetworkBehaviour
{
    [SerializeField] GhostEscapeTrigger escapeTrigger;
    //audio clips
    [Header("audio")]
    [SerializeField] AudioClip crashSound;
    [SerializeField] AudioClip powerVehicleSound;
    [SerializeField] AudioClip unpoweringCarSound;
    [SerializeField] AudioClip engineStartSuccessSound;
    [SerializeField] AudioClip engineStopSound;

    [SerializeField] AudioSource engineStart;
    [SerializeField] AudioSource engineLoop;
    [SerializeField] AudioSource movingLoop;

    //internal components
    private NetworkPlayerController _playerController;
    private PlayerCamera _playerCamera;
    private GameObject _playerObject;

    [Header("components")]
    [SerializeField] CarFixArea fixArea;
    [SerializeField] List<Renderer> dashboardComponents;
    [SerializeField] List<Light> lightSources;
    [SerializeField] List<Light> carHeadlights;
    [SerializeField] List<Renderer> carHeadlightRenderers;
    [SerializeField] GarageDoor gDoor;
    [SerializeField] FuelTank fuelTank;
    [SerializeField] List<Door> doors; 
    public Rigidbody carRB;

    //public
    [Header("misc")]
    [SyncVar] public bool isSealed;
    [SyncVar] public bool hasWheel;

    public float carMoveSpeed;
    public float carMaxMoveSpeed;
    private float _carMoveSpeedTemp;

    public float fuelLevel;

    //input system
    private PlayerControls _controls;
    private InputActionMap _carControlsMap;
    private PlayerControls.CarActions _carActions;

    [SerializeField] CarSeat passenger;
    [SerializeField] CarSeat driver;

    //ui
    [SerializeField] Canvas uiCanvas;

    [SerializeField] TextMeshProUGUI powerText;
    [SerializeField] TextMeshProUGUI startText;
    [SerializeField] TextMeshProUGUI moveText;

    //internal
    private const string keyWord = "_EMISSION";
    private CarSeat _playerSeat;

    private bool _isPowered;
    private bool _hasStarted;
    private bool _isMoveHold;
    bool _isReversedMoveHold;


    private void Awake()
    {
        _controls = new();
        _carActions = _controls.Car;

        _carActions.Power.canceled += PowerVehicle;

        _carActions.Start.performed += StartCarAttemp;
        _carActions.Start.canceled += StartCarAttempCancelled;

        _carActions.Move.performed += MovePerformed;
        //_carActions.Move.canceled += MoveCanceled;

        //_carActions.ReverseMove.performed += ReverseMovePerformed;
        //_carActions.ReverseMove.canceled += ReverseMoveCalceled;

        DisableRendererKeyWord(dashboardComponents);
        DisableRendererKeyWord(carHeadlightRenderers);
        //_playerSeat.Type = CarSeat.SeatType.None;

    }

    #region input controls

    private void ReverseMoveCalceled(InputAction.CallbackContext obj)
    {
        StopCar();
    }
    private void ReverseMovePerformed(InputAction.CallbackContext obj)
    {
        _isMoveHold = false;
        _isReversedMoveHold = true;

        MoveCar(true);
    }


    private void MovePerformed(InputAction.CallbackContext context)
    {
        _isMoveHold = true;
        _isReversedMoveHold = false;

        MoveCar(false);
    }
    //private void MoveCanceled(InputAction.CallbackContext context)
    //{
    //    StopCar();
    //}

    private void StartCarAttemp(InputAction.CallbackContext obj)
    {
        StartCarEngineAttemp();
    }
    private void StartCarAttempCancelled(InputAction.CallbackContext obj)
    {
        StartCarEngineCancelled();
    }


    private void PowerVehicle(InputAction.CallbackContext obj)
    {
        if (!_playerController.hasAuthority) return;

        CmdPowerCar();

        if (!_isPowered)
        {
            powerText.color = Color.green;
            //UIManager.Instance.ShowNorayInteractionAlternate("START <color=#0affae>[HOLD]</color>", Keys.Space);
            startText.color = Color.white;
            _carActions.Start.Enable();
        }
        else
        {

            powerText.color = Color.white;
            startText.color = Color.gray;
            moveText.color = Color.gray;

            //UIManager.Instance.HideAlternateRayInteraction();

            _carActions.Start.Disable();
            _carActions.Move.Disable();

            if (_hasStarted)
            {
                //UIManager.Instance.HideAlternateRayInteraction();
            }
        }
    }


    private void EnableControls(CarSeat carSeat)
    {
        if (carSeat.Type == CarSeat.SeatType.Driver)
        {
            uiCanvas.enabled = true;

            if (_isPowered && !_hasStarted)
            {
                startText.color = Color.white;
                _carActions.Start.Enable();
                //UIManager.Instance.ShowNorayInteractionAlternate("START <color=#0affae>[HOLD]</color>", Keys.Space);
            }
            else if (_hasStarted)
            {
                moveText.color = Color.white;
                CheckDriveAvailability();
               // UIManager.Instance.HideAlternateRayInteraction();
                //UIManager.Instance.ShowNorayInteractionAlternate("MOVE <color=#0affae>[HOLD]</color>", Keys.W);
            }

            _carActions.Power.Enable();

           // UIManager.Instance.ShowNoRayInteraction("POWER ON/OFF", Keys.F);
        }

        _playerController.DisablePlayerControls();

    }

    private void DisableControls(CarSeat carSeat)
    {
        if (carSeat.Type == CarSeat.SeatType.Driver)
        {
            uiCanvas.enabled = false;
            _carActions.Disable();

            startText.color = new Color(155, 155, 155);
            moveText.color = new Color(155, 155, 155);

            UIManager.Instance.HideNoRayInteraction();
            UIManager.Instance.HideAlternateRayInteraction();
        }

        _playerController.EnablePlayerControls();

    }


    #endregion

    #region functionality

    #region enter car c/s

    public void EnterCar(NetworkPlayerController player, CarSeat seat)
    {
        if (!player.hasAuthority) return;

        if (player.isInCar)
        {
            ChangeSeatAvailability();

            if (_playerSeat.Type == CarSeat.SeatType.Driver)
            {
                uiCanvas.enabled = false;
                DisableControls(_playerSeat);
            }
            else
            {
                uiCanvas.enabled = true;
                EnableControls(_playerSeat);
            }
        }
        else
        {
            player.Sit();
        }

        player.inventoryActionMap.Disable();

        _playerController = player;
        _playerCamera = player.GetComponent<PlayerCamera>();
        _playerObject = player.gameObject;

        player.isInCar = true;
        _playerSeat = seat;

        //if(_playerSeat.Type == CarSeat.SeatType.Driver)
        //{
        //    if (_hasStarted)
        //    {
        //        CheckDriveAvailability();
        //    }
        //}

        Item itemWithControls = Inventory.Instance.GetMainItem(_playerController);

        if (itemWithControls != null && itemWithControls is IHasControls)
        {
            (itemWithControls as IHasControls).DisableItemControls();
        }



        EnableControls(seat);
        ChangeSeatAvailability();

        CmdEnterCar(_playerController, _playerObject, _playerSeat);

    }

    [Command(requiresAuthority = false)]
    void CmdEnterCar(NetworkPlayerController _controller, GameObject _playerObject, CarSeat _seat)
    {
        RpcEnterCar(_controller, _playerObject, _seat);
    }
    [ClientRpc]
    void RpcEnterCar(NetworkPlayerController _controller, GameObject _playerObject, CarSeat _seat)
    {
        _controller.controller.enabled = false;
        _playerObject.transform.SetParent(_seat.transform);
        _playerObject.transform.localPosition = Vector3.zero;
        _playerObject.transform.localRotation = Quaternion.Euler(Vector3.zero);

        _controller.inventory.localScale = Vector3.zero;
    }

    #endregion

    #region exit car c/s

    public void ExitCar()
    {
        if (!_playerController.hasAuthority) return;

        _playerController.isInCar = false;

        _playerSeat.FreeOrTakeCarSeat();
        _playerSeat.carSeatExit.EnableOrDisableExitCollider();
        _playerCamera = null;

        DisableControls(_playerSeat);

        _playerController.inventoryActionMap.Enable();

        _playerController.ExitSitting();

        CmdExitCar(_playerController, _playerObject, _playerSeat);

        Item itemWithControls = Inventory.Instance.GetMainItem(_playerController);

        if (itemWithControls != null && itemWithControls is IHasControls)
        {
            (itemWithControls as IHasControls).ReturnControls();
        }

       
    }

    [Command(requiresAuthority = false)]
    void CmdExitCar(NetworkPlayerController _controller, GameObject _playerObject, CarSeat _carSeat)
    {
        RpcExitCar(_controller, _playerObject, _carSeat);
    }
    [ClientRpc]
    void RpcExitCar(NetworkPlayerController _controller, GameObject _playerObject, CarSeat _carSeat)
    {
        _playerObject.transform.parent = _carSeat.carSeatExit.transform;
        _playerObject.transform.position = _carSeat.carSeatExit.transform.position;
        _controller.controller.enabled = true;

        _controller.inventory.localScale = new Vector3(1, 1, 1);
    }

    #endregion

    #region power car c/s

    [Command(requiresAuthority = false)]
    void CmdPowerCar()
    {
        RpcPowerCar();
    }
    [ClientRpc]
    void RpcPowerCar()
    {
        if (!_isPowered)
        {
            AudioSource.PlayClipAtPoint(powerVehicleSound, transform.position);
            _isPowered = true;
        }
        else
        {
            _isPowered = false;
            AudioSource.PlayClipAtPoint(unpoweringCarSound, transform.position);

            if (_hasStarted)
            {
                StopCarEngine();
                _hasStarted = false;
            }
        }

        PowerDashboardElements();

    }

    #endregion

    #region start car engine

    private void StartCarEngine()
    {
        if (_playerSeat != null && _playerSeat.Type == CarSeat.SeatType.Driver)
        {
            UIManager.Instance.HideAlternateRayInteraction();
            startText.color = Color.green;
            moveText.color = Color.white;
            //UIManager.Instance.ShowNorayInteractionAlternate("MOVE <color=#0affae>[HOLD]</color>", Keys.W);

            //if (!hasWheel && fixArea.isWheelInstalled)
            //{
            //    UIManager.Instance.Message("I should tighten up the wheel a bit.");
            //    UIManager.Instance.Message("There has to be a Lug Wrench nearby..");

            //    return;
            //}
            //else if (!fixArea.isWheelInstalled)
            //{
            //    UIManager.Instance.Message("One wheel is missing.");
            //    UIManager.Instance.Message("I guess this won't be a trouble installing it.");

            //    return;
            //}
            //else if (fixArea.isCarJackInstalled)
            //{
            //    UIManager.Instance.Message("Need to remove the Car Jack before I move on.");

            //    return;
            //}

            //_carActions.Move.Enable();
            //_carActions.ReverseMove.Enable();

            CheckDriveAvailability();

        }


        _hasStarted = true;
        AudioSource.PlayClipAtPoint(engineStartSuccessSound, transform.position);
        engineLoop.Play();

        EnableRendererKeyWord(carHeadlightRenderers);

        foreach (Light light in carHeadlights)
        {
            light.enabled = true;
        }

    }

    #endregion

    #region stop car engine

    private void StopCarEngine()
    {
        if (_playerSeat != null && _playerSeat.Type == CarSeat.SeatType.Driver)
        {
            _carActions.Move.Disable();
            _carActions.ReverseMove.Disable();
        }

        foreach (Light light in carHeadlights)
        {
            light.enabled = false;
        }

        DisableRendererKeyWord(carHeadlightRenderers);
        engineLoop.Stop();
        AudioSource.PlayClipAtPoint(engineStopSound, transform.position);

    }

    #endregion

    #region push car back c/s

  

    #endregion

    #region move car c/s

    public void MoveCar(bool reversed)
    {
        CmdMoveCar(reversed);
    }
    [Command(requiresAuthority = false)]
    void CmdMoveCar(bool reversed)
    {
        RpcMoveCar(reversed);
    }
    [ClientRpc]
    void RpcMoveCar(bool reversed)
    {
        if (reversed)
        {
            _isMoveHold = false;
            _isReversedMoveHold = true;
        }
        else
        {
            _isMoveHold = true;
            _isReversedMoveHold = false;
        }

        StartCoroutine(MoveCarCoroutine(reversed));
    }

    #endregion

    #region stop car c/s

    public void StopCar()
    {
        CmdStopCar();
    }
    [Command(requiresAuthority = false)]
    void CmdStopCar()
    {
        RpcStopCar();
    }
    [ClientRpc]
    void RpcStopCar()
    {
        _isMoveHold = false;
        _isReversedMoveHold = false;
    }

    #endregion

    #region start car engine attemp

    public void StartCarEngineAttemp()
    {
        CmdStartCarEngineAttemp();
    }
    [Command(requiresAuthority = false)]
    void CmdStartCarEngineAttemp()
    {
        RpcStartCarEngineAttemp();
    }
    [ClientRpc]
    void RpcStartCarEngineAttemp()
    {
        StartCoroutine(StartEngineCoroutine());
    }

    #endregion

    #region start car engine attemp cancelled

    public void StartCarEngineCancelled()
    {
        CmdStartCarEngineAttempCancelled();
    }
    [Command(requiresAuthority = false)]
    void CmdStartCarEngineAttempCancelled()
    {
        RpcStartCarEngineAttempCancelled();
    }
    [ClientRpc]
    void RpcStartCarEngineAttempCancelled()
    {
        StopAllCoroutines();
        engineStart.Stop();
    }

    #endregion

    public void UnlockCarDoors()
    {
        foreach(Door door in doors)
        {
            door.isSealed = false;
        }
    }

    #endregion

    private void ChangeSeatAvailability()
    {
        _playerSeat.carSeatExit.EnableOrDisableExitCollider();
        _playerSeat.FreeOrTakeCarSeat();
    }

    private void PowerDashboardElements()
    {
        if (_isPowered)
        {
            EnableRendererKeyWord(dashboardComponents);

            foreach (Light light in lightSources)
            {
                light.enabled = true;
            }
        }
        else
        {
            DisableRendererKeyWord(dashboardComponents);

            foreach (Light light in lightSources)
            {
                light.enabled = false;
            }
        }
    }


    #region coroutines

    private IEnumerator StartEngineCoroutine()
    {
        engineStart.Play();

        if (fuelLevel > fuelTank.maxCapacity / 2)
        {
            yield return new WaitForSeconds(3);
            engineStart.Stop();
            StartCarEngine();
        }
        else
        {
            yield return new WaitForSeconds(6f);
            UIManager.Instance.Message("noFuel", "noFuel_A");
        }
    }

    private IEnumerator MoveCarCoroutine(bool isReversed)
    {
        float _moveSpeed = isReversed ? -carMoveSpeed / 1.5f : carMoveSpeed;

        while (_isMoveHold || _isReversedMoveHold)
        {
            if (!movingLoop.isPlaying)
            {
                movingLoop.Play();
            }

            carRB.AddForce(Vector3.forward * _moveSpeed, ForceMode.Acceleration);

            yield return new WaitForSeconds(.02f);
        }

        movingLoop.Stop();

    }

    #endregion


    //misc
    public void DisableRendererKeyWord(List<Renderer> list)
    {
        foreach (Renderer rend in list)
        {
            rend.material.DisableKeyword(keyWord);
        }
    }

    public void EnableRendererKeyWord(List<Renderer> list)
    {
        foreach (Renderer rend in list)
        {
            rend.material.EnableKeyword(keyWord);
        }
    }


    #region push car c/s

    [Command (requiresAuthority = false)]
    void PushCarBackCmd(Vector3 direction, float forceMultiplier, bool impact)
    {
        PushCarBackRpc(direction, forceMultiplier, impact);
    }
    [ClientRpc]
    void PushCarBackRpc(Vector3 direction, float forceMultiplier, bool impact)
    {
        PushCarBack(direction, forceMultiplier, impact);
    }

    void PushCarBack(Vector3 direction, float forceMultiplier, bool impact)
    {
        carRB.linearVelocity = Vector3.zero;
        _isMoveHold = false;

        if (_playerCamera != null && impact)
        {
            _playerCamera.ShakeCamera(.1f, 1.5f, 1f);
        }

        carRB.AddForce(direction * forceMultiplier, ForceMode.Impulse);

        if (impact)
        {
            gDoor.PlayImpactSound();
        }
    }

    #endregion

    #region garage weaken

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("GarageDoor"))
        {
            if (_playerSeat != null && _playerSeat.Type == CarSeat.SeatType.Driver)
            {
                if (!gDoor.isUnlocked)
                {
                    UIManager.Instance.Message("noBreak", "noBreak_A");
                    PushCarBackCmd(Vector3.back, 15, true);
                    return;
                }

                if (gDoor.collisionsAmount > 1)
                {
                    _isMoveHold = false;
                }

                WeakenDoorCmd();
            }
            else if(driver.isTaken == false)
            {
                PushCarBack(Vector3.back, 10, true);
            }
        }
    }

    [Command(requiresAuthority = false)]
    private void WeakenDoorCmd()
    {
        WeakenDoorRpc();
    }

    [ClientRpc]
    private void WeakenDoorRpc()
    {
        switch (gDoor.collisionsAmount)
        {
            case 3: UIManager.Instance.Message("try1", "try1_A"); break;
            case 2: UIManager.Instance.Message("try2", "try2_A");
                escapeTrigger.Activate();
                break;
            case 1: UIManager.Instance.Message("try3", "try3_A"); break;
        }

        if (gDoor.collisionsAmount == 1)
        {
            PushCarBack(Vector3.back, 15, true);
            gDoor.garageParentRB.isKinematic = false;
            gDoor.garageParentRB.mass = .01f;
            gDoor.thisRB.mass = .01f;

            gDoor.environment.SetActive(true);

            gDoor.collisionsAmount--;
        }
        else if (gDoor.collisionsAmount > 1)
        {
            _isMoveHold = false;
            PushCarBack(Vector3.back, 15, true);
            gDoor.collisionsAmount--;
        }
        else if (gDoor.collisionsAmount == 0)
        {
            RenderSettings.fogDensity = 0.03f;
            gDoor.carRain.Play();
            gDoor.PlayImpactSound();
        }
    }

    #endregion


    bool CheckDriveAvailability()
    {
        if (!hasWheel && fixArea.isWheelInstalled)
        {
            UIManager.Instance.Message("missingLug", "missingLug_A");
            //UIManager.Instance.Message("There has to be a Lug Wrench nearby..");

            return false;
        }
        else if (!fixArea.isWheelInstalled)
        {
            UIManager.Instance.Message("missingWheel", "missingWheel_A");
            //UIManager.Instance.Message("I guess this won't be a trouble installing it.");

            return false;
        }
        else if (fixArea.isCarJackInstalled)
        {
            UIManager.Instance.Message("removeJack", "removeJack_A");

            return false;
        }

        _carActions.Move.Enable();
        //_carActions.ReverseMove.Enable();

        PushCarBackCmd(Vector3.back, 3, false);

        return true;
    }

    private void OnDisable()
    {
        _controls.Disable();
    }

}
