using Steamworks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TipPanel : MonoBehaviour
{
    [SerializeField] GameObject holder;
    [SerializeField] Animation bgAnimation;
    [SerializeField] Animation mainImgAnimation;


    public void ShowPanel()
    {
        holder.SetActive(true);
        bgAnimation.Play();
        mainImgAnimation.Play();
    }

    public void ClosePanel()
    {
        holder.SetActive(false);
    }
}
