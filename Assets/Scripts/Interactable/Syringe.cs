using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;
using static UnityEngine.UI.GridLayoutGroup;

public class Syringe : Item
{
    [SyncVar] public int uses;

    private Ray _ray;
    private bool _didHit;
    private RaycastHit _impactedObject;
    private bool _isShown;

    private NetworkPlayerController _ownerCopy;
    public float interactRange = 2;
    [SerializeField] LayerMask layerMask;

    [SerializeField] AudioClip injectSound;
    [SerializeField] AudioSource audioSource;

    public override void TakeItem(NetworkPlayerController owner)
    {
        base.TakeItem(owner);
        _ownerCopy = owner;
        owner.keyObjectlayerMask = LayerMask.GetMask("KeyObject","Revive");

        if (!owner.hasAuthority) return;

        StartCoroutine(SyringeCoroutine());
    }

    public override void OnDropItem()
    {
        _ownerCopy.keyObjectlayerMask = LayerMask.GetMask("KeyObject");
        base.OnDropItem();
        StopAllCoroutines();
    }

    IEnumerator SyringeCoroutine()
    {
        while (true)
        {
            _ray = _ownerCopy.playerCamera.ViewportPointToRay(new Vector3(.5f, .5f, 0));
            _didHit = Physics.Raycast(_ray, out _impactedObject, interactRange, layerMask);

            if (_didHit)
            {
                if (_impactedObject.collider.gameObject.tag == "Respawn")
                {
                    UIManager.Instance.ShowInteractOption(UIManager.Instance.UIRayToolText[5]);
                    _isShown = true;
                }
            }
            else if (_isShown == true)
            {
                _isShown = false;
                UIManager.Instance.HideInteractOption();
            }

            yield return null;
        }
    }

    public void Use(NetworkPlayerController player)
    {
        UseCmd(player);

        SteamUserStats.SetAchievement("INJECTOR_USE");
        SteamUserStats.StoreStats();
    }

    [Command (requiresAuthority = false)]
    void UseCmd(NetworkPlayerController player)
    {
        uses--;
        UseRpc();
        ReviveCmd(player);
    }

    [ClientRpc]
    void UseRpc()
    {
        audioSource.PlayOneShot(injectSound);
    }

    [Command(requiresAuthority = false)]
    void ReviveCmd(NetworkPlayerController player)
    {
        player.isAlive = true;
        ReviveRpc(player);
    }
    [ClientRpc]
    void ReviveRpc(NetworkPlayerController player)
    {
        player.Revive(player);
    }

}
