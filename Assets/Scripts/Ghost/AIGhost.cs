using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Mirror;
using Steamworks;

public class AIGhost : Mirror.NetworkBehaviour
{
    private NavMeshAgent _aiGhostNavMesh;
    public Animator _ghostAnimator;
    [SerializeField] Transform raycastPosition;

    [Header("Audio")]
    [SerializeField] AudioSource stepsSource;
    [SerializeField] AudioSource singingSource;
    [SerializeField] AudioSource walkingModeSource;

    [SerializeField] AudioClip impactSound;
    [SerializeField] List<AudioClip> ghostHuntSounds;
    [SerializeField] List<AudioClip> appearanceSoundClips;

    [Header("Components")]
    [SerializeField] GameObject ghostModel;
    [SerializeField] Collider aura;

    [Header("Ghost settings")]
    public float hearingRange;
    public float sightRange;
    public float huntDelay = 100f;

    public float _crawlMultiplier;

    private Vector3 _lastPlayerPosition;
    private RaycastHit _hit;

    public bool _isInWalkMode;

    private bool _isInSight;
    private bool _isSpotted;
    private bool _hasBeenSpotted;
    private bool _isIdle = true;
    private bool _isCrawling;
    private float _timeElapsed = 0;

    [SyncVar] public bool _isInUnharmedMode = true;
    private bool _hasScreamed;

    //test

    private int _patrolRouteIndex;

    [Header("Layer masks")]
    [SerializeField] LayerMask playerMask;

    private GameObject _playerToChase;
    private List<Transform> _patrolPoints;
    Collider[] colliders;
    Collider _collider;

    public bool hasNightmareAnimation;

    int _crawlSpeedMultiplierHash = Animator.StringToHash("crawlSpeedMultiplier");
    int _speedMultiplierHash = Animator.StringToHash("speedMultiplier");
    int _isDifficultHash = Animator.StringToHash("isDifficult");
    bool hasBeenKilled = false;
    bool killedCrucifix = false;

    //Raycast
    private RaycastHit hitInfo;


    [ServerCallback]
    private void ServerStart()
    { 
        if(GhostEvent.Instance.isRaged) return;
        int randomNumber = Random.Range(0, 100);
        int clipIndex;

        if (randomNumber < 37)
        {
            clipIndex = 0;
        }
        else if (randomNumber > 36 && randomNumber < 60)
        {
            clipIndex = 1;
        }
        else
        {
            clipIndex = 2;
        }

        RpcSetGhostHuntSound(clipIndex);

    }

    [ClientRpc]
    void RpcSetGhostHuntSound(int clipIndex)
    {
        singingSource.clip = ghostHuntSounds[clipIndex];
        singingSource.Play();
    }

    private void Awake()
    {
        _collider = GetComponent<Collider>();
        _isInUnharmedMode = true;
    }

    private void Start()
    {
        _isInUnharmedMode = true;
        _patrolPoints = GhostEvent.Instance.patrolPoints;
        _patrolRouteIndex = Random.Range(0, _patrolPoints.Count);

        _aiGhostNavMesh = GetComponent<NavMeshAgent>();
        _ghostAnimator = ghostModel.GetComponent<Animator>();

        _aiGhostNavMesh.speed *= SetupPanel.LevelSettings.GhostSpeed * GhostEvent.Instance.extraSpeedMultiplier;
        _ghostAnimator.SetFloat(_speedMultiplierHash,
            _ghostAnimator.GetFloat(_speedMultiplierHash) * SetupPanel.LevelSettings.GhostSpeed * GhostEvent.Instance.extraSpeedMultiplier);

        _ghostAnimator.SetBool("isIdle", true);

        if (hasNightmareAnimation)
        {
            if (SetupPanel.LevelSettings.DifficultyType == LevelDifficultyType.Nightmate || GhostEvent.Instance._huntCount > 3)
            {
                _ghostAnimator.SetBool(_isDifficultHash, true);
            }
        }

        ServerStart();

    }
    private void Update()
    {
        _timeElapsed += Time.deltaTime;

        if (_timeElapsed <= huntDelay && _isIdle)
        {
            return;
        }
        else if (!_isCrawling && _timeElapsed >= huntDelay)
        {
            _isInUnharmedMode = false;
            _isCrawling = true;
            _isIdle = false;
            _collider.enabled = true;

            _ghostAnimator.SetBool("isIdle", false);

        }

        colliders = Physics.OverlapSphere(raycastPosition.position, hearingRange, playerMask);

        

        if (colliders.Length != 0)
        {
            _isInSight = true;
        }
        else _isInSight = false;

        if (_isInSight || _hasBeenSpotted)
        {
            if (colliders.Length > 0)
                _playerToChase = colliders[0].gameObject;

            ChasePlayer();
        }
        if (!_isSpotted && !_hasBeenSpotted)
        {
            Patrol();
        }

    }

    #region AI
    private void ChasePlayer()
    {
        float angle = Vector3.Angle(raycastPosition.forward, _playerToChase.transform.position - raycastPosition.position);

        if (Physics.Raycast(raycastPosition.position, _playerToChase.transform.position - raycastPosition.position, out _hit))
        {
            if (_hit.collider.tag == "Player" && _hit.distance <= sightRange || _isSpotted && _hit.distance <= sightRange && colliders.Length > 0)
            {
                _isSpotted = true;
                _hasBeenSpotted = true;

                if (!_isInUnharmedMode && !_hasScreamed)
                {
                    //if (Random.Range(0, 5) == 1)
                    //{
                    //    spottingSource.Play();
                    //}

                    //_hasScreamed = true;
                }

                _lastPlayerPosition = _playerToChase.transform.localPosition;
                _aiGhostNavMesh.SetDestination(_playerToChase.transform.position);
            }
            else if (_hasBeenSpotted)
            {
                _isSpotted = false;

                float distance = Vector2.Distance(new Vector2(raycastPosition.position.x, raycastPosition.position.z), new Vector2(_lastPlayerPosition.x, _lastPlayerPosition.z));

                _aiGhostNavMesh.SetDestination(_lastPlayerPosition);

                if (distance < 1.5f)
                {
                    _hasBeenSpotted = false;

                    _patrolRouteIndex = GetPatrolRouteIndex();
                    Patrol();
                }
            }
        }
    }

    private void Patrol()
    {
        _aiGhostNavMesh.SetDestination(_patrolPoints[_patrolRouteIndex].position);

        if (Vector3.Distance(raycastPosition.position, _patrolPoints[_patrolRouteIndex].position) < 3)
        {
            _patrolRouteIndex = GetPatrolRouteIndex();
        }
    }

    private int GetPatrolRouteIndex()
    {
        Random.InitState((int)System.DateTime.Now.Ticks);
        _patrolRouteIndex = Random.Range(0, _patrolPoints.Count);

        return _patrolRouteIndex;
    }

    public void SetPatrolRoute(int index)
    {
        _patrolRouteIndex = index;
    }

    #endregion

    public void Mute()
    {
        //stepsSource.Stop();
        singingSource.volume = 0;

        _isInUnharmedMode = true;

        int index = Random.Range(0, appearanceSoundClips.Count + 1);

        //if (index < appearanceSoundClips.Count)
        //{
        //    AudioSource.PlayClipAtPoint(appearanceSoundClips[index], transform.position, .8f);
        //}
    }

    public void SetWalkingMode()
    {
        aura.gameObject.transform.localScale /=1.2f;
        //_aiGhostNavMesh.speed /= 1.3f;
       // walkingModeSource.Play();
        //playerMask = 0;
        //ghostModel.SetActive(false);
        //aura.enabled = false;
        _isInWalkMode = true;
        _isInUnharmedMode = true;
        walkingModeSource.volume /= 1.3f;
        Mute();
        //StartCoroutine(GenerateFootstepsCoroutine());

    }

    public void EndWalkingMode()
    {
        StopCoroutine("GenerateFootstepsCoroutine");
        stepsSource.Stop();
    }

    private void KillPlayer(NetworkPlayerController player)
    {
        player.Die();
        _isSpotted = false;
        _hasBeenSpotted = false;

    }

    private void OnTriggerEnter(Collider collision)
    {

        if (collision.gameObject.tag == "Player")
        {
            if (_isInUnharmedMode) return;

            NetworkPlayerController player = collision.gameObject.GetComponent<NetworkPlayerController>();

            Debug.Log("trigger");

            if(Inventory.Instance.GetMainItem(player) is Crucifix)
            {
                if(!killedCrucifix){

                killedCrucifix = true;
                Crucifix crucifix = Inventory.Instance.GetMainItem(player) as Crucifix;

                if(crucifix.uses > 0 && !_isInUnharmedMode && !_isInWalkMode)
                {
                    if (NetworkPlayerController.NetworkPlayer.isServer)
                    {
                        crucifix.Damage(); 
                    }

                    Destroy(gameObject);
                    return;
                }
                }
            }

            if (_isInWalkMode)
            {
                AudioSource.PlayClipAtPoint(impactSound, raycastPosition.position);

                player.EnterScaredMode(TimeRange.Short, ScaredModeProperty.ShockMode);

                Destroy(gameObject);

                return;
            }

            if (!player.hasAuthority) return;

                if(!hasBeenKilled){
                     KillPlayer(player);
                     hasBeenKilled = true;
                }
               
        }
        if (collision.gameObject.tag == "ClosedDoor")
        {

            if (!NetworkPlayerController.NetworkPlayer.isServer) return;

            Door door = collision.gameObject.GetComponent<Door>();

            if (door is LockerDoor)
            {
                door.isSealed = false;
                door.OpenCloseDoorCommand();
            }
            else if (door is Locker)
            {
                door.isSealed = false;
                door.OpenCloseDoorCommand();
            }
            else
            {
                if (!door.isSealed)
                {
                    door.OpenCloseDoorCommand();
                }
            }
        }

    }

}
