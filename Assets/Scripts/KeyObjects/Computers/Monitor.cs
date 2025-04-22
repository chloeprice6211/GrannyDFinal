using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monitor : MonoBehaviour
{
    [SerializeField] Material screen;
    [SerializeField] Light lightSource;
    [SerializeField] Renderer _renderer;

    const string keyWord = "_EMISSION";

    
    private void Awake()
    {
        _renderer.materials[2].DisableKeyword(keyWord);
    }

    public void TurnOnScreen()
    {
        _renderer.materials[2].color = Color.white;
        _renderer.materials[2].EnableKeyword(keyWord);
        lightSource.enabled = true;
    }

    public void TurnOffScreen()
    {
        lightSource.enabled = false;
        _renderer.materials[2].color = Color.black;
        _renderer.materials[2].DisableKeyword(keyWord);
    }
}
