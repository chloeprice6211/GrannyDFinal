using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnPoint : MonoBehaviour
{
    // Start is called before the first frame update
    private void Awake() => PlayerSpawnManager.AddSpawnPoint(transform);
    private void OnDestroy() => PlayerSpawnManager.RemoveSpawnPoint(transform);
}
