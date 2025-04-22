using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbienceScript : MonoBehaviour
{
    public bool isInside;
    public float rate = 0.05f;

    Coroutine _changeRoutine;
    bool _isOngoing;

    public void Change(AudioSource toIncrease, AudioSource toDecrease)
    {
        if (_changeRoutine != null && _isOngoing)
            StopCoroutine(_changeRoutine);

        _changeRoutine = StartCoroutine(ChangeRoutine(toIncrease, toDecrease));
    }

    IEnumerator ChangeRoutine(AudioSource toIncrease, AudioSource toDecrease)
    {
        _isOngoing = true;

        while(toIncrease.volume <= 0.9f)
        {
            toIncrease.volume += rate;
            toDecrease.volume -= rate;

            yield return new WaitForSeconds(.05f);
        }

        _isOngoing = false;
    }
}
