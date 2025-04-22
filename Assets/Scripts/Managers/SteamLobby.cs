using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using Mirror;

public class SteamLobby : MonoBehaviour
{
    static public SteamLobby instance;

    public LobbyInvitePanel invitePanel;

    protected Callback<LobbyCreated_t> lobbyCreatedCallback;
    protected Callback<GameLobbyJoinRequested_t> gameJoinRequestCallback;
    protected Callback<LobbyEnter_t> lobbyEnterCallback;
    protected Callback<LobbyInvite_t> lobbyGameInviteCallback;

    public CSteamID _lobbyId;
    private const string HostAddressKey = "HostAddress";
    private const string MapNameKey = "MapName";
    private const string LvlKey = "LVL";
    private const string FootageIndexKey = "FootageIndex";
    public static string lvl;

    private void Start()
    {
        instance = this;

        lobbyCreatedCallback = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        gameJoinRequestCallback = Callback<GameLobbyJoinRequested_t>.Create(OnJoinRequest);
        lobbyEnterCallback = Callback<LobbyEnter_t>.Create(OnLobbyEnter);
        lobbyGameInviteCallback = Callback<LobbyInvite_t>.Create(OnLobbyGameInvite);
    }

    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        if (callback.m_eResult != EResult.k_EResultOK)
        {
            return;
        }

        _lobbyId = new CSteamID(callback.m_ulSteamIDLobby);

        SteamMatchmaking.SetLobbyData(
            new CSteamID(callback.m_ulSteamIDLobby),
            HostAddressKey,
            SteamUser.GetSteamID().ToString());

        SteamMatchmaking.SetLobbyData(
            new CSteamID(callback.m_ulSteamIDLobby),
            MapNameKey,
            SetupPanel.Instance.levelToGo.levelName);

        SteamMatchmaking.SetLobbyData(
             new CSteamID(callback.m_ulSteamIDLobby),
            FootageIndexKey,
            SetupPanel.Instance.levelToGo.pinboardIndex.ToString());

        SetupPanel.Instance.ApplyLevelName(SetupPanel.Instance.levelToGo.levelName, SetupPanel.Instance.levelToGo.pinboardIndex.ToString());
            

        CustomNetworkManager.Instance.StartHost();
    }

    private void OnJoinRequest(GameLobbyJoinRequested_t callback)
    {
        JoinLobby(callback.m_steamIDLobby);
    }

    private void OnLobbyEnter(LobbyEnter_t callback)
    {
        if (NetworkServer.active)
        {
            Debug.LogError("Entering with an active lobby");
            return;
        }

        //Debug.LogError("Entering with an active lobby");

        string hostAddress = SteamMatchmaking.GetLobbyData(
            new CSteamID(callback.m_ulSteamIDLobby),
            HostAddressKey
            );

        lvl = SteamMatchmaking.GetLobbyData(
            new CSteamID(callback.m_ulSteamIDLobby),
            LvlKey
            );

        SetupPanel.Instance.ApplyLevelName(SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), MapNameKey), SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), FootageIndexKey));

        NetworkManager.singleton.networkAddress = hostAddress;
        NetworkManager.singleton.StartClient();

        Debug.LogWarning("lobby enter");
    }

    private void OnLobbyGameInvite(LobbyInvite_t callback)
    {
        //Debug.LogError("YOU ARE INVITED");
        invitePanel.ShowInvitePanel(callback);
    }

    public void JoinLobbyViaPanel(CSteamID lobbyID)
    {
        JoinLobby(lobbyID);
    }

    private void JoinLobby(CSteamID lobbyID)
    {
        if (NetworkServer.active)
        {
            CustomNetworkManager.Instance.StopHost();
            StartCoroutine(JoinRequestCoroutine());
        }
        else
        {
            SteamMatchmaking.JoinLobby(lobbyID);
        }

        IEnumerator JoinRequestCoroutine()
        {
            yield return new WaitForSeconds(1.5f);
            SteamMatchmaking.JoinLobby(lobbyID);
        }

        Debug.LogWarning("lobby join");
    }

}
