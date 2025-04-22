using System.Collections.Generic;
using NUnit.Framework.Constraints;
using UnityEngine;

public class GhostRoofBehaviour : MonoBehaviour
{
    public Animation roofAnim;
    public GameObject ghostGO;

    public List<GhostTriggerRoof> rooftriggers;

    float _timer;
    float _zoneActiveCountdown;
    bool _hasBeenEnabled;

    public float additionalDurr = 0f;
    void Update(){
        _timer += Time.deltaTime;

        if(_timer >=400){
            _zoneActiveCountdown+=Time.deltaTime;

            if(!_hasBeenEnabled){
            foreach(GhostTriggerRoof rooftrigger in rooftriggers){
                rooftrigger._collider.enabled = true;
            }
            _hasBeenEnabled = true;
            }

            if(_zoneActiveCountdown >=60){

                foreach(GhostTriggerRoof rooftrigger in rooftriggers){
                rooftrigger._collider.enabled = false;

                _timer = 0f;
                _zoneActiveCountdown = 0f;
                _hasBeenEnabled = false;
            }
            }
        }
    }

    public void TriggerGhostRoofWalk(){
        roofAnim.Play();
        ghostGO.SetActive(true);
        Invoke(nameof(DestroyGhost),roofAnim.clip.length + additionalDurr);
    }

    void DestroyGhost(){
        ghostGO.SetActive(false);
    }
}
