using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RatingStar : MonoBehaviour
{
    [SerializeField] Sprite outline;
    [SerializeField] Sprite full;

    [SerializeField] Image image;

    public void Enable()
    {
        image.sprite = full;
    }
    public void Disable()
    {
        image.sprite = outline;
    }
}
