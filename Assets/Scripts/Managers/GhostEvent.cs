using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.Events;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine.Rendering.Universal;
using Steamworks;

public class GhostEvent : NetworkBehaviour
{
    private enum GhostMode
    {
        WalkingMode,
        AppearMode
    }
    //and this is another for test rebase
    //here we are commiting changes
    [Header("Objects ghost can interact with")]
    [SerializeField] List<Door> doors;
    [SerializeField] List<Lamp> lamps;
    [SerializeField] List<GameObject> miscObjects;
    [SerializeField] List<Fusebox> fuseboxes;
    [SerializeField] List<LightSwitch> lightSwitchers;
    [SerializeField] List<ThrowableItem> throwableItems;
    [SerializeField] List<LVL2Switcher> newSwitchers;
    public List<IGhostInteractable> miscInteractable = new();
    public int itemGroupsAmount;

    private IGhostInteractable _ghostInteractableObject;

    [SerializeField] List<AudioClip> ambientGhostSounds;

    [SerializeField] List<Shadow> shadows;

    #region Ghost actions adjustments

    [Header("Hunt settings")]
    public float huntStartTimeRangeFrom;
    public float huntStartTimeRangeTo;
    private float _timeToHunt;

    [Header("Ghost Random Action settings")]
    public float ghostActionTimeRangeFrom;
    public float ghostActionTimeRangeTo;
    private float _timeToGhostAction;

    [Header("Random Ambient Sound settings")]
    public float ambientSoundAppearTimeRangeFrom;
    public float ambientSoundAppearTimeRangeTo;
    private float _timeToAmbientSoundAppearance;

    [Header("Ghost Appearance settings")]
    public float ghostAppearanceTimeRangeFrom;
    public float ghostAppearanceTimeRangeTo;
    private float _timeToGhostAppearance;

    [Header("Ghost safe walking settings")]
    public float ghostWalkTimeRangeFrom;
    public float ghostWalkTimeRangeTo;
    private float _timeToGhostWalk;

    [Header("Ghost Corner Appearance settings")]
    public float cornerAppearanceTimeRangeFrom;
    public float cornerAppearanceTimeRangeTo;
    float _timeToCornerAppearance;

    #endregion

    [Header("Misc")]
    public float huntEventActiveTime;
    public float ghostWalkingActiveTime;
    public float cooldownTime;

    [Header("Events")]
    public UnityEvent OnHuntStart;
    public UnityEvent OnHuntEnd;

    public UnityEvent OnRageStart;
    public UnityEvent OnRageEnd;

    public int _huntCount;
    public float extraSpeedMultiplier  =1f;

    [Header("Ghost components")]
    [SerializeField] Transform ghostHolder;
    public List<Transform> patrolPoints;
    public List<Transform> ghostCornerPoints;
    [SerializeField] List<Transform> ghostSpawnPoints;
    [SerializeField] List<Transform> shadowSpawnPoints;
    [SerializeField] AIGhost ghostPrefab;
    private AIGhost ghostInstance;

    private bool _isActive;
    [HideInInspector]
    public bool canStart;

    //countdowns
    private float _countdown;
    private float _huntEventCountdown;
    private float _ghostActionCountdown;
    private float _appearanceCountdown;
    private float _cornerAppearanceCountdown;
    private float _ambientSoundAppearCountdown;
    private float _ghostWalkCountdown;
    private float _eventCooldownCountdown;

    [SyncVar] public bool isRaged;

    public AudioSource rageSource;
    public List<AudioClip> rageStartClips;
    public List<AudioClip> rageDurationClips;

    private static GhostEvent _instance;
    public static GhostEvent Instance
    {
        get
        {
            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;
    }


    [ServerCallback]
    private void Start()
    {
        SetupEventTimings(SetupPanel.LevelSettings);
        _eventCooldownCountdown = cooldownTime;
    }

    private void SetupEventTimings(LevelSettings Settings)
    {
        huntEventActiveTime = LevelSettings.ghostHuntDuration;

        ghostActionTimeRangeFrom = Settings.GhostActionTime;
        ghostActionTimeRangeTo = ghostActionTimeRangeFrom + (ghostActionTimeRangeFrom);

        ambientSoundAppearTimeRangeFrom = Settings.GhostAmbientSound;
        ambientSoundAppearTimeRangeTo = ambientSoundAppearTimeRangeFrom + (ambientSoundAppearTimeRangeFrom);

        ghostAppearanceTimeRangeFrom = Settings.GhostAppearanceTime;
        ghostAppearanceTimeRangeTo = ghostAppearanceTimeRangeFrom + (ghostAppearanceTimeRangeFrom * 2);
        
        ghostWalkTimeRangeFrom = Settings.GhostWalkModeTime;
        ghostWalkTimeRangeTo = ghostWalkTimeRangeFrom + (ghostWalkTimeRangeFrom * 1.3f);

        huntStartTimeRangeFrom = Settings.GhostHuntTime;
        huntStartTimeRangeTo = huntStartTimeRangeFrom + (huntStartTimeRangeFrom * 2);

        cornerAppearanceTimeRangeFrom = Settings.GhostCornerAppearanceTime;
        cornerAppearanceTimeRangeTo = cornerAppearanceTimeRangeFrom + (cornerAppearanceTimeRangeFrom * 2);

        cooldownTime = Settings.GhostEventCooldown;

        _timeToGhostAction = GenerateNumberFromRange(ghostActionTimeRangeFrom, ghostActionTimeRangeTo);
        _timeToHunt = GenerateNumberFromRange(huntStartTimeRangeFrom, huntStartTimeRangeTo);
        _timeToAmbientSoundAppearance = GenerateNumberFromRange(ambientSoundAppearTimeRangeFrom, ambientSoundAppearTimeRangeTo);
        _timeToGhostAppearance = GenerateNumberFromRange(ghostAppearanceTimeRangeFrom, ghostAppearanceTimeRangeTo);
        _timeToGhostWalk = GenerateNumberFromRange(ghostWalkTimeRangeFrom, ghostWalkTimeRangeTo);
        _timeToCornerAppearance = GenerateNumberFromRange(cornerAppearanceTimeRangeFrom, cornerAppearanceTimeRangeTo);

        if(miscObjects.Count> 0) ConvertList(miscObjects);

    }

    [ServerCallback]
    void Update()
    {
        if (!canStart) return;

        _ghostActionCountdown += Time.deltaTime;
        _huntEventCountdown += Time.deltaTime;
        _appearanceCountdown += Time.deltaTime;
        _ambientSoundAppearCountdown += Time.deltaTime;
        _ghostWalkCountdown += Time.deltaTime;
        _cornerAppearanceCountdown += Time.deltaTime;
        _countdown +=Time.deltaTime;

        if (_ghostActionCountdown >= _timeToGhostAction)
        {
            PerformGhostAction();
        }
        if (_ambientSoundAppearCountdown >= _timeToAmbientSoundAppearance)
        {
            int clipIndex = Random.Range(0, ambientGhostSounds.Count);
            Debug.Log(clipIndex);
            Transform soundPosition = ghostSpawnPoints[Random.Range(0, ghostSpawnPoints.Count)];
            PlayAmbientSoundclip(clipIndex, soundPosition);
        }

        if (_eventCooldownCountdown > 0)
        {
            _eventCooldownCountdown -= Time.deltaTime;
            return;
        }
      
        if (_huntEventCountdown >= _timeToHunt && !_isActive)
        {
            if(_countdown > 800){
StartHuntEvent(true);
_countdown = 0;
            }
            else{
                StartHuntEvent(false);
            }
            
            _isActive = true;
           //_eventCooldownCountdown = cooldownTime;
            return;
        }
        if (_appearanceCountdown >= _timeToGhostAppearance && !_isActive && Random.Range(0,2) == 1)
        {
            MakeGhostAppearance();
            _isActive = true;
            //_eventCooldownCountdown = cooldownTime/ 2;
            return;
        }

        if(_cornerAppearanceCountdown >= _timeToCornerAppearance && !_isActive && ghostCornerPoints != null &&ghostCornerPoints.Count >0)
        {
            CornerAppearance();
            _isActive = true;
            return;
        }

        if (_ghostWalkCountdown >= _timeToGhostWalk && !_isActive && Random.Range(0, 4) == 3)
        {
            StartGhostWalking();
            _isActive = true;

            return;
        }
    }

    #region Ghost functionality/environment actions

    //hunt
    public IEnumerator HuntEventCoroutine()
    {
        OnHuntStart?.Invoke();

        if(isRaged){

         for(int a= 0;a<shadowSpawnPoints.Count-1;a++){
            shadows[a].gameObject.SetActive(true);
         }

         }

        if (isServer)
        {
            ghostInstance = Instantiate(ghostPrefab, GetSpawnPoint(), Quaternion.identity);
            NetworkServer.Spawn(ghostInstance.gameObject);

            yield return new WaitForSeconds(huntEventActiveTime);

            EndHuntEvent();
        }

       
    }

    [QFSW.QC.Command("hunt")]
    [ClientRpc]
    public void StartHuntEvent(bool IsRaged)
    {
        _huntCount++;
        isRaged = IsRaged;
        if(extraSpeedMultiplier <= 1.2f)
            extraSpeedMultiplier += .05f;

            if (IsRaged){
                rageSource.PlayOneShot(rageStartClips[Random.Range(0, rageStartClips.Count)]);
                rageSource.PlayOneShot(rageDurationClips[Random.Range(0, rageDurationClips.Count)]);
                OnRageStart?.Invoke();
            }

        StartCoroutine(HuntEventCoroutine());
    }

    [ClientRpc]
    private void EndHuntEvent()
    {
        if (isServer)
        {
            if(cooldownTime > 1)
            {
                cooldownTime-=.5f;
            }

            _eventCooldownCountdown = cooldownTime;
            _huntEventCountdown = 0;
            _isActive = false;

            _timeToHunt = GenerateNumberFromRange(huntStartTimeRangeFrom, huntStartTimeRangeTo);

            if(ghostInstance!= null)
            {
                NetworkServer.Destroy(ghostInstance.gameObject);
            } 
        }

        if(isRaged){
 for(int a= 0;a<shadowSpawnPoints.Count-1;a++){
            shadows[a].gameObject.SetActive(false);
         }
         OnRageEnd?.Invoke();
            isRaged =false;
        
        }
        

       
            

        OnHuntEnd?.Invoke();
    }

    //appearance
    public void MakeGhostAppearance()
    {
        if (isServer)
        {
            ghostInstance = Instantiate(ghostPrefab, GetSpawnPoint(), Quaternion.identity);
            NetworkServer.Spawn(ghostInstance.gameObject);

            _eventCooldownCountdown = cooldownTime / 1.2f;
            _isActive = true;

            SetupGhost(ghostInstance, GhostMode.AppearMode);

            Invoke("EndGhostAppearance", 4f);
        }  
    }

    public void EndGhostAppearance()
    {
        NetworkServer.Destroy(ghostInstance.gameObject);
        _timeToGhostAppearance = GenerateNumberFromRange(ghostAppearanceTimeRangeFrom, ghostAppearanceTimeRangeTo);

        _appearanceCountdown = 0;
        _isActive = false;
    }

    public void EndCornerAppearance()
    {
        NetworkServer.Destroy(ghostInstance.gameObject);
        _timeToCornerAppearance = GenerateNumberFromRange(cornerAppearanceTimeRangeFrom, cornerAppearanceTimeRangeTo);

        _cornerAppearanceCountdown = 0;
        _isActive = false;
    }

    //corner appearance
    public void CornerAppearance()
    {
        if (isServer)
        {
            ghostInstance = Instantiate(ghostPrefab, ghostCornerPoints[Random.Range(0, ghostCornerPoints.Count)].position, Quaternion.identity);
            NetworkServer.Spawn(ghostInstance.gameObject);
            _eventCooldownCountdown = cooldownTime / 1.2f;
            _isActive = true;

            SetupGhost(ghostInstance, GhostMode.AppearMode);

            Invoke("EndCornerAppearance", 12f);
        }
    }


    //walking
    public IEnumerator GhostWalkingCoroutine()
    {
        if (isServer)
        {
            _isActive = true;

            ghostInstance = Instantiate(ghostPrefab, GetSpawnPoint(), Quaternion.identity);
            NetworkServer.Spawn(ghostInstance.gameObject);

            SetupGhost(ghostInstance, GhostMode.WalkingMode);

            yield return new WaitForSeconds(ghostWalkingActiveTime);
            EndGhostWalking();
        }


    }

    public void StartGhostWalking()
    {
        StartCoroutine(GhostWalkingCoroutine());
    }

    public void EndGhostWalking()
    {
        if (cooldownTime > 1)
        {
            cooldownTime-=.5f;
        }

        _eventCooldownCountdown = cooldownTime;
        _isActive = false;

        if(ghostInstance != null)
        {
            NetworkServer.Destroy(ghostInstance.gameObject);
        }

        _timeToGhostWalk = GenerateNumberFromRange(ghostWalkTimeRangeFrom, ghostActionTimeRangeTo);
        _ghostWalkCountdown = 0;
    }


    //random ambient sound
    [ClientRpc]
    public void PlayAmbientSoundclip(int clipIndex, Transform soundPosition)
    {
        _ambientSoundAppearCountdown = 0;
        AudioSource.PlayClipAtPoint(ambientGhostSounds[clipIndex], soundPosition.position);
    }


    public void PerformGhostAction()
    {
        int randomActionIndex = Random.Range(0, itemGroupsAmount);
        int randomObjectIndex;

        switch (randomActionIndex)
        {
            case 0:
                Perform(doors.ToList<IGhostInteractable>());
                break;
            case 1:
                if (lamps.Count > 0)
                {
                    Perform(lamps.ToList<IGhostInteractable>());
                }
                break;
            case 2:
                Perform(fuseboxes.ToList<IGhostInteractable>());
                break;
            case 3:
                if(miscInteractable.Count > 0)
                {
                    Perform(miscInteractable);
                }
                break;
            case 4:
                if(lightSwitchers.Count > 0)
                {
                    Perform(lightSwitchers.ToList<IGhostInteractable>());
                }
                break;
            case 5:
                if(newSwitchers.Count > 0)
                {
                    Perform(newSwitchers.ToList<IGhostInteractable>());
                }
                break;
            case 6:
                if(throwableItems.Count > 0)
                {
                    Perform(throwableItems.ToList<IGhostInteractable>());
                }
                break;
        }

        _ghostActionCountdown = 0;
        _timeToGhostAction = GenerateNumberFromRange(ghostActionTimeRangeFrom, ghostActionTimeRangeTo);

        void Perform(List<IGhostInteractable> list)
        {
            randomObjectIndex = Random.Range(0, list.Count);
            _ghostInteractableObject = list[randomObjectIndex];
            _ghostInteractableObject.PerformGhostInteraction();
        }
    }

    [ClientRpc]
    private void SetupGhost(AIGhost ghost, GhostMode mode)
    {
        if (mode is GhostMode.WalkingMode)
        {
            ghost.SetWalkingMode();
        }
        else if (mode is GhostMode.AppearMode)
        {
            ghost.Mute();
            ghost.huntDelay = 12;
        }
    }

    #endregion

    public Vector3 GetSpawnPoint()
    {
        return ghostSpawnPoints[Random.Range(0, ghostSpawnPoints.Count)].position;
    }

    private float GenerateNumberFromRange(float from, float to)
    {
        return Random.Range(from, to);
    }

    private void ConvertList(List<GameObject> objects)
    {
        foreach (GameObject obj in objects)
        {
            miscInteractable.Add(obj.GetComponent<IGhostInteractable>());
        }
    }

}
