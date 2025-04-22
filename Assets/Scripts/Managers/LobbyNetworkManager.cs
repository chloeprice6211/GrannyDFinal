using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyNetworkManager : NetworkBehaviour
{
    static public LobbyNetworkManager instance;
    public bool hasConnectedCompleted;

    private void Awake()
    {
        instance = this;
    }


    [Command(requiresAuthority = false)]
    public void CmdEnablePreSceneFade()
    {
        RpcEnablePreSceneFade();
    }

    [ClientRpc]
    void RpcEnablePreSceneFade()
    {
        if (isClientOnly)
        {
            StartCoroutine(LoadSceneRoutine());
        }
    }

    IEnumerator LoadSceneRoutine()
    {
        MenuManager.instance.screenFadeAnimation.Play("ExitFade");
        yield return new WaitForSeconds(2f);
        MenuManager.instance.clientLoadingPanel.SetActive(true);
    }


    [Command(requiresAuthority = false)]
    public void CmdChangeDifficulty(LevelSettings levelSettings)
    {
        RpcChangeDifficulty(levelSettings);
    }
    [ClientRpc]
    void RpcChangeDifficulty(LevelSettings levelSettings)
    {
        SetupPanel.LevelSettings = levelSettings;
        hasConnectedCompleted = true;
    }
}
