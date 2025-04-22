 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using Steamworks;

public class CustomNetworkManager : NetworkManager
{
    [Header("Prefabs")]
    [SerializeField] private GameObject playerListPrefab = null;
    [SerializeField] private GameObject gamePlayerPrefab = null;
    [SerializeField] private LobbyPlayer lobbyPlayerPrefab;

    public List<Vector3> spawnPositions = new();
    public List<Quaternion> spawnRotations = new();
    public bool isSteam;


    public List<LobbyPlayer> LobbyPlayers = new List<LobbyPlayer>();
    public List<NetworkPlayerController> GamePlayers = new List<NetworkPlayerController>();
    [HideInInspector]
    public static int playersCounter = 0;

    int spawnIndex = 0;

    public static CustomNetworkManager Instance
    {
        get
        {
            return singleton as CustomNetworkManager;
        }
    }

    public override void Start()
    {
        base.Start();
    }

    public override void OnClientDisconnect()
    {
        base.OnClientDisconnect();

        LobbyPlayers.Clear();
        GamePlayers.Clear();

        StopClient();

        if(SceneManager.GetActiveScene().name == "Menu")
        {
            UpdateLobbyPlayers();
            Debug.Log("client disconencted");
        }
        
    }

    #region Network events

    public override void OnStartHost()
    {
        //Debug.Log("host stareed");
        if (LobbyPlayers.Count > 0)
        {
            foreach (LobbyPlayer lp in LobbyPlayers)
            {
                NetworkServer.DestroyPlayerForConnection(lp.GetComponent<NetworkIdentity>().connectionToClient);
            }
        }

        base.OnStartHost();
        playersCounter = 0;

        GamePlayers.Clear();
        LobbyPlayers.Clear();
        spawnIndex = 0;
    }
    public override void OnStartClient()
    {
        base.OnStartClient();
        playersCounter = 0;
        GamePlayers.Clear();
    }



    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        LobbyPlayer playerInstance = Instantiate(lobbyPlayerPrefab);
        //Debug.Log("added");

        NetworkServer.AddPlayerForConnection(conn, playerInstance.gameObject);

        if (playersCounter == 0)
        {
            playerInstance.playerName = "Chloe";
        }
        else if (playersCounter == 1)
        {
            
            playerInstance.playerName = "Jane";
        }
        else
        {
            playerInstance.playerName = "Kelly";
        }

        UpdateLobbyPlayers();

        playersCounter++;
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        if (SceneManager.GetActiveScene().name == "Menu")
        {
            LobbyPlayers.Remove(conn.identity.GetComponent<LobbyPlayer>());
            MyLobby.instance.UpdateClients(conn.identity.GetComponent<LobbyPlayer>());

            base.OnServerDisconnect(conn);
            

            playersCounter--;
        }
    }

    public override void OnClientConnect()
    {
        base.OnClientConnect();
        UpdateLobbyPlayers();
    }

    public override void OnStopHost()
    {
        if (SceneManager.GetActiveScene().name == "Hollowgate")
        {
            Destroy(gameObject);
            SceneManager.LoadScene("Menu");
        }

        if(SceneManager.GetActiveScene().name == "TutorialScene")
        {
            Destroy(gameObject);
        }
        playersCounter = 0;
        GamePlayers.Clear();
        LobbyPlayers.Clear();
        base.OnStopHost();
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        playersCounter = 0;
        GamePlayers.Clear();
        LobbyPlayers.Clear();
    }

    #endregion

    #region Lobby

    public void UpdateLobbyPlayers()
    {
        if (SceneManager.GetActiveScene().name != "Menu") return;

        foreach (Transform child in MenuManager.instance.playersHolder)
        {
            Destroy(child.gameObject);
        }

        foreach (LobbyPlayer player in LobbyPlayers)
        {
            GameObject playerItemListObject = Instantiate(playerListPrefab, MenuManager.instance.playersHolder);
            PlayerItemList playerItem = playerItemListObject.GetComponent<PlayerItemList>();

            playerItem.PlayerName = player.playerName;

        }

        SetupPanel.Instance.UpdatePlayerCount(LobbyPlayers.Count);

    }

    #endregion

    #region Spawning Game Player

    public override void ServerChangeScene(string newSceneName)
    {
        base.ServerChangeScene(newSceneName);
    }

    public void StartGame()
    {
        ServerChangeScene("SampleScene");
    }

    public void SpawnPlayer(NetworkConnectionToClient conn)
    {
        GameObject playerInstance = Instantiate(gamePlayerPrefab, spawnPositions[spawnIndex], spawnRotations[spawnIndex]);
        playerInstance.name += Random.Range(0, 100).ToString();
        NetworkServer.Spawn(playerInstance, conn);
        spawnIndex++;
    }

    public override void OnServerReady(NetworkConnectionToClient conn)
    {
        if (SceneManager.GetActiveScene().name == "Hollowgate")

        {
            base.OnServerReady(conn);
            SpawnPlayer(conn);
        }
    }

    #endregion

}
