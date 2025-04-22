using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyPlayer : Mirror.NetworkBehaviour
{ 
    [SyncVar] public string playerName = "default";
    [SyncVar] public int elementIndex;

    private PlayerControls lobbyControls;
    private CustomNetworkManager netManager;

    private CustomNetworkManager NetManager
    {
        get
        {
            if (netManager != null)
            {
                return netManager;
            }
            return netManager = NetworkManager.singleton as CustomNetworkManager;
        }
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public override void OnStartClient()
    {
        if (isClientOnly && isLocalPlayer)
        {
            //playerName = SteamFriends.GetPersonaName();
            MenuManager.instance.ChangeMenuPoint(TargetMenuPoint.Coop);
        }

        NetManager.LobbyPlayers.Add(this);
        NetManager.UpdateLobbyPlayers();

    }

    public override void OnStopLocalPlayer()
    {
        NetManager.LobbyPlayers.Clear();
        base.OnStopLocalPlayer();
    }

    public override void OnStopClient()
    {
        if (SceneManager.GetActiveScene().name == "Menu")
        {
            if (isLocalPlayer)
            {
                base.OnStopClient();
                if (SetupPanel.Instance.virtualCamera.gameObject.activeInHierarchy)
                {
                    SetupPanel.Instance.ReturnFromItemsGuide();
                }
                MenuManager.instance.ChangeMenuPoint(TargetMenuPoint.Menu);
            }
        }
    }

    private void Start()
    {
        if (!isLocalPlayer) return;
    }

    public void SendLoggerMessage(string message)
    {
        if (isServer) SendLoggerMessageRpc(message, playerName);
        else SendLoggerMessageCommand(message, playerName);
    }


    [Command]
    public void SendLoggerMessageCommand(string message, string playerName)
    {
        if (string.IsNullOrEmpty(message)) return;

        SendLoggerMessageRpc(message.Trim(), playerName);
    }

    [Command]
    public void StartNewScene()
    {
        SceneManager.LoadScene("SampleScene");
    }

    [ClientRpc]
    public void SendLoggerMessageRpc(string message, string playerName)
    {
        Logger.instance.LogPlayerMessage(playerName, message);
    }

    private void OnDestroy()
    {
       // lobbyControls.Disable();
    }
}
