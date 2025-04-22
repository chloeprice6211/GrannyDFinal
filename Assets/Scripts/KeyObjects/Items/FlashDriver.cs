using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class FlashDriver : UtilityItem
{
    [SerializeField] Renderer _renderer;
    [SerializeField] Material glowingMaterial;
    [SerializeField] Light lightSource;
    [SerializeField] AudioClip insertFlashSound;

    [Command (requiresAuthority = false)]
    public void TurnFlashOnCommand()
    {
        TurnFlashOnRpc();
    }
    [ClientRpc]
    private void TurnFlashOnRpc()
    {
        lightSource.enabled = true;
        _renderer.material = glowingMaterial;
        AudioSource.PlayClipAtPoint(insertFlashSound, transform.position);

        GetComponent<Collider>().enabled = false;
    }

    public void TurnFlashOn()
    {
        //OnDropItem();
        TurnFlashOnCommand();
    }

    public override void OnInsert()
    {
        //transform.parent.localScale = parentScale;
        lightSource.enabled = true;
        _renderer.material = glowingMaterial;
        AudioSource.PlayClipAtPoint(insertFlashSound, transform.position);
    }

}
