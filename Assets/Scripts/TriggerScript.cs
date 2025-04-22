using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerScript : MonoBehaviour
{
    [SerializeField] GameObject objectToTrigger;
    ITriggered _iTriggeredObject;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (objectToTrigger.TryGetComponent(out _iTriggeredObject))
        {
            _iTriggeredObject.OnTrigger();
            Destroy(gameObject);
        }
        
    }
}
