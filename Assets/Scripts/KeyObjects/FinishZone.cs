using Steamworks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishZone : MonoBehaviour
{
    // Start is called before the first frame update

    public EndingAudio endingAudio;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Car" || other.tag == "Player")
        {
            if (SceneManager.GetActiveScene().name == "SampleScene")
            {
                if (SetupPanel.LevelSettings.DifficultyType == LevelDifficultyType.Easy)
                {
                    if (SteamManager.Initialized)
                    {
                        SteamUserStats.SetAchievement("BROWNS_ESCAPE");
                        SteamUserStats.StoreStats();
                    }
                    
                }
                else if (SetupPanel.LevelSettings.DifficultyType == LevelDifficultyType.Normal)
                {
                    SteamUserStats.SetAchievement("BROWNS_ESCAPE_NORMAL");
                    SteamUserStats.StoreStats();
                }
                else if (SetupPanel.LevelSettings.DifficultyType == LevelDifficultyType.Nightmate)
                {
                    SteamUserStats.SetAchievement("BROWNS_ESCAPE_NIGHTMARE");
                    SteamUserStats.StoreStats();
                }

            }
            else if(SceneManager.GetActiveScene().name == "LVL2")
            {
                if (SetupPanel.LevelSettings.DifficultyType == LevelDifficultyType.Easy)
                {
                    SteamUserStats.SetAchievement("MANSION_ESCAPE");
                    SteamUserStats.StoreStats();
                }
                else if (SetupPanel.LevelSettings.DifficultyType == LevelDifficultyType.Normal)
                {
                    SteamUserStats.SetAchievement("MANSION_ESCAPE_NORMAL");
                    SteamUserStats.StoreStats();
                }
                else if (SetupPanel.LevelSettings.DifficultyType == LevelDifficultyType.Nightmate)
                {
                    SteamUserStats.SetAchievement("MANSION_ESCAPE_NIGHTMARE");
                    SteamUserStats.StoreStats();
                }
            }
            else if (SceneManager.GetActiveScene().name == "LVL3")
            {
                if (SetupPanel.LevelSettings.DifficultyType == LevelDifficultyType.Easy)
                {
                    SteamUserStats.SetAchievement("BEAVER_ESCAPE");
                    SteamUserStats.StoreStats();
                }
                else if (SetupPanel.LevelSettings.DifficultyType == LevelDifficultyType.Normal)
                {
                    SteamUserStats.SetAchievement("BEAVER_ESCAPE_NORMAL");
                    SteamUserStats.StoreStats();
                }
                else if (SetupPanel.LevelSettings.DifficultyType == LevelDifficultyType.Nightmate)
                {
                    SteamUserStats.SetAchievement("BEAVER_ESCAPE_NIGHTMARE");
                    SteamUserStats.StoreStats();
                }
            }

            GameManager.Instance.GameOver(Ending.Escaped);

            if(endingAudio!=null){
                endingAudio.PlayEndingAudio();
            }
            endingAudio.PlayEndingAudio();
            Destroy(gameObject);
        }
    }
}
