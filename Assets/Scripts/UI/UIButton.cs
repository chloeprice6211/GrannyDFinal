using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIButton : MonoBehaviour
{
    [SerializeField] protected List<Sprite> backgrounds;
    [SerializeField] protected Image buttonImage;
    [SerializeField] protected TextMeshProUGUI buttonText;

    [SerializeField] protected AudioSource audioSource;
    [SerializeField] protected List<AudioClip> audioClips;

    [SerializeField] protected Color buttonTextHoverColor;
    [SerializeField] protected Color buttonTextRegularColor;

    [SerializeField] protected Image carett;

    public bool hasState;

    public virtual void OnClick()
    {
        audioSource.PlayOneShot(audioClips[0]);
        Debug.Log("primary class triggered");
    }

    public virtual void OnMouseHover()
    {
        buttonImage.sprite = backgrounds[0];
        buttonText.color = buttonTextHoverColor;
        audioSource.PlayOneShot(audioClips[0]);
        
    }

    public virtual void OnMouseLeave()
    {
        buttonImage.sprite = backgrounds[1];
        buttonText.color = buttonTextRegularColor;
    }
}
