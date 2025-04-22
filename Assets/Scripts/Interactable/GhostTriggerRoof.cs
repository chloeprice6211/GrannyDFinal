using UnityEngine;

public class GhostTriggerRoof : MonoBehaviour
{
    public GhostRoofBehaviour bh;
    public Collider _collider;
    bool _canBeTriggered = true;
    bool _isHunted;

    void Start(){
        _collider = GetComponent<Collider>();
        //GhostEvent.Instance.OnHuntStart.AddListener(OnHunt);
        //GhostEvent.Instance.OnHuntEnd.AddListener(OnHuntEnd);
    }
    
    void OnTriggerEnter(Collider other){
        if(_isHunted) return;
        if(!_canBeTriggered) return;
        bh.TriggerGhostRoofWalk();
        _canBeTriggered = false;

       
    }

     void OnHunt(){
        _isHunted = true;
        }
        void OnHuntEnd(){
_isHunted = false;
        }
}
