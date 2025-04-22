using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SwitcherButton : MonoBehaviour
{
    [SerializeField] List<Sprite> backgrounds;
    [SerializeField] Image buttonImage;
    [SerializeField] TextMeshProUGUI buttonText;
    [SerializeField] AudioSource audioSource;
    [SerializeField] List<AudioClip> audioClips;
    [SerializeField] Animation appearAnimation;
    [SerializeField] Animation corretLoopAnimation;
    [SerializeField] Image carett;

    bool type;

    private void OnEnable()
    {
        
    }

    public void OnButtonClick()
    {
        audioSource.PlayOneShot(audioClips[0]);
    }

    public void OnHover()
    {
        buttonText.color = Color.green;
        audioSource.PlayOneShot(audioClips[1]);

    }

    public void OnMouseLeave()
    {
        buttonText.color = Color.white;
    }
}
