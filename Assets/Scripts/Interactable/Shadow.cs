using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Shadow : MonoBehaviour
{
    float opacityValue;
    public bool hasNightmareAnimation;
    public List<Renderer> shadowRenderers = new();
    private NavMeshAgent _aiGhostNavMesh;
    private Animator _ghostAnimator;
    [SerializeField] Transform raycastPosition;

    [Header("Components")]
    [SerializeField] GameObject ghostModel;

    [Header("Ghost settings")]

    Transform initialPosition;

    public float huntDelay = 100f;
    private bool _isIdle = true;
    private bool _isCrawling;
    private float _timeElapsed = 0;
    bool isMoving = false;

    //test

    private int _patrolRouteIndex;

    [Header("Layer masks")]

    private List<Transform> _patrolPoints;

    int _crawlSpeedMultiplierHash = Animator.StringToHash("crawlSpeedMultiplier");
    int _speedMultiplierHash = Animator.StringToHash("speedMultiplier");
    int _isDifficultHash = Animator.StringToHash("isDifficult");

    void OnEnable(){
//  _patrolPoints = GhostEvent.Instance.patrolPoints;
//         _patrolRouteIndex = Random.Range(0, _patrolPoints.Count);

//         _aiGhostNavMesh = GetComponent<NavMeshAgent>();
//         _ghostAnimator = ghostModel.GetComponent<Animator>();

//         _aiGhostNavMesh.speed *= SetupPanel.LevelSettings.GhostSpeed * GhostEvent.Instance.extraSpeedMultiplier;
//         _ghostAnimator.SetFloat(_speedMultiplierHash,
//             _ghostAnimator.GetFloat(_speedMultiplierHash) * SetupPanel.LevelSettings.GhostSpeed * GhostEvent.Instance.extraSpeedMultiplier);

//         _ghostAnimator.SetBool("isIdle", true);

//         if (hasNightmareAnimation && SetupPanel.LevelSettings.DifficultyType == LevelDifficultyType.Nightmate)
//         {
//                 _ghostAnimator.SetBool(_isDifficultHash, true);
            
//         }

//         isMoving = Random.Range(0,100) > 80;
//         _timeElapsed = 0;
    }
    private void Start()
    {
       

    }
    private void Update()
    {
        _timeElapsed += Time.deltaTime;

        if (_timeElapsed <= huntDelay+1 && _isIdle)
        {
            return;
        }
        if(!isMoving) return;
        else if (!_isCrawling && _timeElapsed >= huntDelay+1)
        {
            _isCrawling = true;
            _isIdle = false;

            _ghostAnimator.SetBool("isIdle", false);

        }
  
            Patrol();

    }

    #region AI

    private void Patrol()
    {
        _aiGhostNavMesh.SetDestination(_patrolPoints[_patrolRouteIndex].position);

        if (Vector3.Distance(raycastPosition.position, _patrolPoints[_patrolRouteIndex].position) < 3)
        {
            Debug.Log("changed patrol");
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

}
