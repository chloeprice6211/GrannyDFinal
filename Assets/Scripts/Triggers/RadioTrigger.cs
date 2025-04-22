using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RadioTrigger : MonoBehaviour
{
    [SerializeField] Radio radio;
    bool canStart = false;

    void Start(){
        StartCoroutine(RadioRoutine());
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!canStart) return;
        if (other.tag == "Player")
        {
            radio.PlayOnTrigger();
        }
    }

    IEnumerator RadioRoutine(){
        yield return new WaitForSeconds(300f);
        canStart = true;

    }
}
