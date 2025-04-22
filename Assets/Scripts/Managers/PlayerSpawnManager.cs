using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerSpawnManager : Mirror.NetworkBehaviour
{
    [SerializeField] GameObject playerPrefab = null;

    private static List<Transform> spawnPoints = new List<Transform>();

    private int nextIndex = 0;

    public static void AddSpawnPoint(Transform transform)
    {
        spawnPoints.Add(transform);
    }   

    public static void RemoveSpawnPoint(Transform transform) => spawnPoints.Remove(transform);

    [Server]
    public void SpawnPlayer(NetworkConnection connection)
    {
        GameObject playerInstance = Instantiate(playerPrefab);
        NetworkServer.Spawn(playerInstance);

        nextIndex++;
    }
}
