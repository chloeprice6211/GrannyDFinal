using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class Level : MonoBehaviour
{
    public string sceneName;
    public Transform cameraPosition;
    public PostProcessProfile volume;
    public List<Transform> spawnPoints;
    public int huntTime;
    public float speedMultiplier;
    public int playerCap;

    public int pinboardIndex;
    public string levelName;
    [TextArea]
    public string levelDescription;

}
