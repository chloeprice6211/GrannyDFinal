using System.Collections.Generic;
using UnityEngine;

public class PoliceYard : MonoBehaviour, ITriggered
{
    public List<SwatOperator> operators;

    public void OnTrigger()
    {
        OnAnimators();
    }

    void OnAnimators(){
        foreach(SwatOperator swat in operators){
            swat.animator.enabled = true;
        }
    }
}
