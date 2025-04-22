using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TVActivationTrigger : MonoBehaviour
{
    [SerializeField] GameObject TVobject;

    bool canStart = true;

    void Start(){
        StartCoroutine(TVTimer());
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!canStart) return;
        if (other.CompareTag("Player"))
        {
            TVobject.SetActive(true);
        }
    }

    IEnumerator TVTimer(){
        yield return new WaitForSeconds(200);
        canStart = true;

    }


}
