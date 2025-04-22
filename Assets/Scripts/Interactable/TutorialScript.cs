using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialScript : MonoBehaviour
{
    public List<AudioSource> audioSources;
    [SerializeField] Animation fade;
    public float[] sourcesMaxVolume;

    private void Start()
    {
        StartCoroutine(AudioSourceIncreaseRoutine());
    }

    IEnumerator AudioSourceIncreaseRoutine()
    {
        while (audioSources[0].volume <= sourcesMaxVolume[0])
        {
            for (int a = 0; a < audioSources.Count; a++)
            {
                audioSources[a].volume += sourcesMaxVolume[a] / 20;
            }
            yield return new WaitForSeconds(.5f);
        }

    }

    IEnumerator AudioSourceDecreaseRoutine()
    {
        fade.Play("EndingFade");
        AsyncOperation asyncOp = SceneManager.LoadSceneAsync("Menu");
        asyncOp.allowSceneActivation = false;

        while (audioSources[0].volume >0)
        {
            for (int a = 0; a < audioSources.Count; a++)
            {
                audioSources[a].volume -= sourcesMaxVolume[a] / 5;
            }
            yield return new WaitForSeconds(.5f);
        }

        CustomNetworkManager.Instance.StopHost();
       
        asyncOp.allowSceneActivation = true;
    }

    public void EndTutorial()
    {
        StartCoroutine(AudioSourceDecreaseRoutine());
    }
}
