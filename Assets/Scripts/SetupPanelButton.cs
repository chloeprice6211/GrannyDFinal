using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetupPanelButton : Button
{
    [SerializeField] AudioClip onHoverSound;
    [SerializeField] AudioClip onClickSound;

    [SerializeField] Renderer cubeRenderer;

    [SerializeField] List<Material> materials;
    [SerializeField] Color color;

    [SerializeField] Light buttonLight;

    public void OnClick()
    {
        MenuManager.instance.PlaySound(onClickSound, .5f);
    }

    public void OnHover()
    {
        cubeRenderer.material = materials[0];
        buttonLight.color = Color.white;
        MenuManager.instance.PlaySound(onHoverSound, .25f);
    }

    public void OnHoverLeave()
    {
        cubeRenderer.material = materials[1];
        buttonLight.color = color;
    }


}
