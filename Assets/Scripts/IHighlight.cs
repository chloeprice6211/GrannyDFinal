using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHighlight
{
    public void Highlight();
    public void DisableHighlight();
    public void ChangeHighlightThickness(float value);
}
