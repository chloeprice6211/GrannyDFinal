using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Steamworks;
using UnityEngine.SceneManagement;

public class LobbyInvitePanel : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI inviterTextField;
    [SerializeField] TextMeshProUGUI countdownTextField;

    [SerializeField] Animation anim;

    [SerializeField] Button acceptButton;
    [SerializeField] Button declineButton;

    [SerializeField] AnimationClip appearClip;
    [SerializeField] AnimationClip dissapearClip;

    [SerializeField] Image timerBG;

    public int inviteActiveDuration;

    float _timeElapsed;

    CSteamID _currentLobbyID;

    public void ShowInvitePanel(LobbyInvite_t lobbyData)
    {
        if (SceneManager.GetActiveScene().name != "Menu") return;

        string senderName = SteamFriends.GetFriendPersonaName(new CSteamID(lobbyData.m_ulSteamIDUser));
        _currentLobbyID = new CSteamID(lobbyData.m_ulSteamIDLobby);
        inviterTextField.text = senderName + " has invited you";

        anim.Play(appearClip.name);

        StartCoroutine(DisplayRoutine());
    }

    public void OnAcceptClick()
    {
        SteamLobby.instance.JoinLobbyViaPanel(_currentLobbyID);
        anim.Play(dissapearClip.name);
        StopCoroutine("DisplayRoutine");
    }

    public void OnDeclineClick()
    {
        anim.Play(dissapearClip.name);
    }

    IEnumerator DisplayRoutine()
    {
        float _duration = inviteActiveDuration;
        timerBG.fillAmount = 1;

        while(_duration >= 0)
        {
            _duration -= Time.deltaTime;
            timerBG.fillAmount = _duration / inviteActiveDuration;
            countdownTextField.text = _duration.ToString("0.0") + "s";

            yield return null;
        }

        anim.Play(dissapearClip.name);
    }

}
