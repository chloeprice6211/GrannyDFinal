using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillEffect : MonoBehaviour
{
    private float _elapsedTime = 0f;
    public float effectDuration = SetupPanel.LevelSettings.PillsEffectDuration;


    private void OnEnable()
    {
        _elapsedTime = 0f;
    }

    private void Update()
    {
        _elapsedTime += Time.deltaTime;

        if(_elapsedTime >= effectDuration)
        {
            gameObject.SetActive(false);
        }
    }
}
