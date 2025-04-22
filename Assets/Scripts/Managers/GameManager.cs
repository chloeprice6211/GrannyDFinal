using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.PostProcessing;
using Steamworks;
using UnityEngine.Localization;

public class GameManager : Mirror.NetworkBehaviour
{
    private static GameManager _instance;
    public List<Note> notes;
    public List<NetworkPlayerController> Players;
    [SyncVar] public int playersAlive;
    public PostProcessProfile volume;
    public PostProcessProfile defaultVolume;

    public Animation ghostObject;
    public GameObject ghostObj;

    [Header("start phrases")]

    public int maxPlayers;

    [TextArea]
    public string lvlHostStartPhrase;
    [TextArea]
    public string lvlHostSecondPhrase;
    [TextArea]
    public string lvlClientStartPhrase;
    [TextArea]
    public string lvlClientSecondPhrase;

    public static GameManager Instance
    {
        get
        {
            return _instance;
        }
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        CmdIncreaseAlivePlayerCount();

    }

    void Awake()
    {
       _instance = this;
        notes = new List<Note>();
        Note.s_collectedCount = 0;
        SetupLevelDifficulty(SetupPanel.LevelSettings);
        MenuManager.OnLocaleInitialized.AddListener(OnLocalizationInit);
        
    }

    void OnLocalizationInit(Locale locale)
    {

    }

    public void GameOver(Ending endingType)
    {
        Ending.ending = endingType;
        UIManager.Instance.ShowEndingPanel(endingType);
    }

    private void SetupLevelDifficulty(LevelSettings Settings)
    {
        //GhostEvent.Instance.ambientSoundAppearTimeRangeFrom = Settings.GhostAmbientSound * 4;
        //GhostEvent.Instance.ambientSoundAppearTimeRangeTo = (Settings.GhostAmbientSound * 4) + (Settings.GhostAmbientSound * 2);

        //GhostEvent.Instance.cooldownTime = Settings.GhostEventCooldown;

        //GhostEvent.Instance.ghostActionTimeRangeFrom = Settings.GhostActionTime * 4;
        //GhostEvent.Instance.ghostActionTimeRangeTo = (Settings.GhostActionTime * 4) + (Settings.GhostActionTime * 2);

        //GhostEvent.Instance.ghostAppearanceTimeRangeFrom = Settings.GhostAppearanceTime * 4;
        //GhostEvent.Instance.ghostAppearanceTimeRangeFrom = (Settings.GhostAppearanceTime * 4) + (Settings.GhostAppearanceTime * 2);

        //GhostEvent.Instance.ghostWalkTimeRangeFrom = Settings.GhostWalkModeTime * 4;
        //GhostEvent.Instance.ghostWalkTimeRangeTo = (Settings.GhostWalkModeTime * 4) + (Settings.GhostWalkModeTime * 2);

        //GhostEvent.Instance.huntStartTimeRangeFrom = Settings.GhostHuntTime * 4;
        //GhostEvent.Instance.huntStartTimeRangeTo = (Settings.GhostHuntTime * 4) + (Settings.GhostHuntTime * 2);


    }


    [Command(requiresAuthority = false)]
    public void CmdReduceAlivePlayerCount()
    {
        playersAlive--;
    }

    [Command(requiresAuthority = false)]
    public void CmdIncreaseAlivePlayerCount()
    {
        playersAlive++;
    }

    [Command(requiresAuthority = false)]
    public void CmdEndGame()
    {
        RpcEndGame();
    }
    [ClientRpc]
    void RpcEndGame()
    {
        StartCoroutine(EndGameRoutine());
    }

    IEnumerator EndGameRoutine()
    {
        UIManager.Instance.fadeAnimation.Play();
        yield return new WaitForSeconds(4f);

        if (NetworkPlayerController.NetworkPlayer.isServer)
        {
            CustomNetworkManager.Instance.StopHost();
        }
    }

}

public struct Ending
{
    public enum EndingType
    {
        None,
        Escaped,
        Died,
        Left
    }
    public static Ending ending;

    public string message;
    public EndingType endingType;
    public string conclusionMessage;

    private static Ending _escapedEnding = new(
        EndingType.Escaped,
        "endingEscaped",
        "endingMessageEscaped");

    private static Ending _deathEnding = new(
        EndingType.Died,
           "endingDied",
        "endingMessageDied");

    private static Ending _leftEnding = new(
        EndingType.Left,
        "endingLeft");

    public static Ending Escaped
    {
        get
        {
            return _escapedEnding;
        }
    }
    public static Ending Death
    {
        get
        {
            return _deathEnding;
        }
    }
    public static Ending Left
    {
        get
        {
            return _leftEnding;
        }
    }

    Ending(EndingType type, string conclusion, string _message = default)
    {
        endingType = type;
        conclusionMessage = conclusion;
        message = _message;
    }
}
