using UnityEngine;

public class trailertrigger : MonoBehaviour
{   bool canstart = true;
    void OnTriggerEnter(Collider other){
        if(!canstart) return;
         Invoke(nameof(AwaitHunt), 3f);
         canstart = false;
    }

    void AwaitHunt(){
        GhostEvent.Instance.StartHuntEvent(true);
    }
}
