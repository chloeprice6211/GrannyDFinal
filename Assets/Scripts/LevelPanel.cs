using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelPanel : MonoBehaviour
{
    [SerializeField] Image lvlPanelBG;
    [SerializeField] GameObject corett;
    [SerializeField] TextMeshProUGUI lvlNameText;

    [SerializeField] AudioClip hoverSound;
    [SerializeField] AudioClip clickSound;

    [SerializeField] AudioSource aSource;

    public Level level;
    public string sceneName;


    public void OnClick()
    {
        AudioSource.PlayClipAtPoint(clickSound, transform.position);
    }

    public void OnMouseHover()
    {
     aSource.PlayOneShot(hoverSound);

        lvlPanelBG.color = Color.green;
        lvlNameText.color = Color.green;

        corett.SetActive(true);
    }

    public void OnMouseLeave()
    {
        lvlPanelBG.color = Color.white;
        lvlNameText.color = Color.white;

        corett.SetActive(false);
    }
}
