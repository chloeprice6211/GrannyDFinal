using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GamePlayer : Mirror.NetworkBehaviour
{ 
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

    private void Update()
    {
        if (!isLocalPlayer) return;

        if (Input.GetKeyDown(KeyCode.X))
        {
            Debug.Log("X");
        }
    }

    public override void OnStartClient()
    {
        DontDestroyOnLoad(gameObject);

        //NetManager.GamePlayers.Add(this);

    }
}
