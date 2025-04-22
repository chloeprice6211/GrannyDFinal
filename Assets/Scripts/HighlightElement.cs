using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightElement : MonoBehaviour
{
    [SerializeField] Outline outline;

    public void Highlight()
    {
        outline.enabled = true;
    }

    public void DisableHighlight()
    {
        outline.enabled = false;
    }
}
