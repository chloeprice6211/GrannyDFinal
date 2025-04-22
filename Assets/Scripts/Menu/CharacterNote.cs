using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class CharacterNote : MonoBehaviour
{
    string noteText = @"It was a very late night and I have no idea what hapenned. I was on my regular camping trip. At some moment, I felt really tired and decided to take a nap... This trip wasn't the same like others. I always had a feeling that I hear someone in the woods. The voice sounded like it was either a child or a woman. Eventually, I woke up at some unknown place.The first thing that came to my mind is someone thought I'd passed out and took me to their place. It was raining outside so it made sense. But things cannot be as good as I wanted them to be...";

    TextMeshProUGUI textContent;

    private float timeElapsed;
    private int currentCharacter = 0;

    void Start()
    {
        textContent = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(SceneManager.GetSceneByName("SampleScene").isLoaded);
        timeElapsed += Time.deltaTime;

        if(timeElapsed > 0.05f)
        {
            if (currentCharacter < noteText.Length)
            {
                textContent.text += noteText[currentCharacter];

                currentCharacter++;

                timeElapsed = 0f;
            }

        }
    }
}
