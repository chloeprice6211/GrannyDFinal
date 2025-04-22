using Mirror;
using Steamworks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuSceneLoader : MonoBehaviour
{
    AsyncOperation loadingOperation;

    [SerializeField] Animation photoSensAnimation;
    [SerializeField] Transform spawnPos;

    bool _hasCompletedTutorial;


    private void Start()
    {
        loadingOperation = SceneManager.LoadSceneAsync("Menu");
        loadingOperation.allowSceneActivation = false;
        //    Destroy(CustomNetworkManager.Instance.gameObject);
        //if(PlayerPrefs.GetInt("tutorial") == 1)
        //{
        //    _hasCompletedTutorial = true;
        //    loadingOperation = SceneManager.LoadSceneAsync("Menu");
        //    Destroy(CustomNetworkManager.Instance.gameObject);
        //}
        //else
        //{
        //    _hasCompletedTutorial = false;
        //    CustomNetworkManager.Instance.StartHost();
        //}

        //CustomNetworkManager.Instance.spawnPositions.Add(spawnPos.position);
        //CustomNetworkManager.Instance.spawnRotations.Add(spawnPos.rotation);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        StartCoroutine(TimingsRoutine()); 
    }

    IEnumerator TimingsRoutine()
    {
        //if (!_hasCompletedTutorial)
        //{
        //    while (!NetworkClient.ready)
        //    {
        //        yield return null;
        //    }

        //    NetworkManager.singleton.ServerChangeScene("TutorialScene");
        //    loadingOperation = NetworkManager.loadingSceneAsync;
        //}

        loadingOperation.allowSceneActivation = false;

        yield return new WaitForSeconds(9f);
        photoSensAnimation.Play();

        yield return new WaitForSeconds(5f);

        loadingOperation.allowSceneActivation = true;

    }

}
