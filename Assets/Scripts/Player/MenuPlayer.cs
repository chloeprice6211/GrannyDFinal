using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using Mirror;

public class MenuPlayer : Mirror.NetworkBehaviour
{
    [SyncVar(hook = nameof(HandleSteamIdUpdate))]
    private ulong steamId;

    public string steamName;

    public void SetSteamId(ulong steamId)
    {
        this.steamId = steamId;
    }

    public void HandleSteamIdUpdate(ulong oldId, ulong newId)
    {
        steamName = SteamFriends.GetPersonaName();
    }
}