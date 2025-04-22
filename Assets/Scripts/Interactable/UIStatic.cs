using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIStatic : MonoBehaviour
{
    public static UIStatic Instance;
    public AudioSource AudioSource;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        Instance = this;
    }


}
