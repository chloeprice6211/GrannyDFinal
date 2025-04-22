using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostFootstep : MonoBehaviour
{
    private float _timeElapsed;

    private void Update()
    {
        _timeElapsed += Time.deltaTime;

        if(_timeElapsed >= 50)
        {
            Destroy(transform.parent.gameObject);
        }
    }
}
