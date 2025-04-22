using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingTriggerScript : MonoBehaviour
{
    NetworkPlayerController _controller;
    [SerializeField] LVL3Ending _endingScript;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Player")) return;


        if (other.gameObject.TryGetComponent(out _controller))
        {
            if (Inventory.Instance.GetMainItem(_controller) != null)
            {
                Item _item = Inventory.Instance.GetMainItem(_controller);

                if(_item is Key && (_item as Key).objectiveType == ObjectiveType.LVL3_Main)
                {
                    _endingScript.OnTrigger();
                    Destroy(gameObject);
                }
            }
        }
    }
}
