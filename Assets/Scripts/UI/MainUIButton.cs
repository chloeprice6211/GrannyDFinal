using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MainUIButton : MonoBehaviour
{
    public enum ButtonType
    {
        Black,
        White
    }
    public bool mustChange;
    public ButtonType type;

    [SerializeField] List<Sprite> backgrounds;
    [SerializeField] Image buttonImage;
    [SerializeField] TextMeshProUGUI buttonText;
    [SerializeField] AudioSource audioSource;
    [SerializeField] List<AudioClip> audioClips;
    [SerializeField] Animation appearAnimation;
    [SerializeField] Animation corretLoopAnimation;
    [SerializeField] Image carett;
    [SerializeField] TextMeshProUGUI hoverTextField;


    private void OnEnable()
    {
        OnMouseLeave();
        appearAnimation.Play();
    }

    public void OnButtonClick()
    {
        if(UIStatic.Instance != null)
        {
            UIStatic.Instance.AudioSource.PlayOneShot(audioClips[0]);
        }
    }

    public void OnHover()
    {
        if (UIStatic.Instance != null)
        {
            UIStatic.Instance.AudioSource.PlayOneShot(audioClips[1]);
        }
        carett.gameObject.SetActive(true);

        if (mustChange) return;

        if (appearAnimation.isPlaying)
        {
            buttonImage.color = Color.white;
            buttonText.color = Color.black;
        }

        if(type == ButtonType.White)
        {
            buttonImage.color = Color.green;
        }

        buttonImage.sprite = backgrounds[0];
        buttonText.color = Color.black;
       
       
    }

    public void OnMouseLeave()
    {
        carett.gameObject.SetActive(false);
        if (mustChange) return; 

        buttonImage.sprite = backgrounds[1];

        if(type == ButtonType.Black)
        {
            buttonText.color = Color.white;
        }
        else
        {
            buttonImage.color = Color.white;
        }
        
        
    }
}
