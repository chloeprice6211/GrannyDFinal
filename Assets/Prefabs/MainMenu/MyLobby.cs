using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;

public class MyLobby : Mirror.NetworkBehaviour
{
    static public MyLobby instance
    {
        get; private set;
    }

    public TextMeshProUGUI testTextField;

    [SerializeField] GameObject parentElement;
    [SerializeField] GameObject playerListPrefab;

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
        if (instance == null)
        {
            instance = this;
        }

    }


    [ClientRpc]
    public void UpdateClients(LobbyPlayer player)
    {
        Debug.Log("updated");

        CustomNetworkManager.Instance.LobbyPlayers.Remove(player);
        CustomNetworkManager.Instance.UpdateLobbyPlayers();
    }
}
