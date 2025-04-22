using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class CableSystem : NetworkBehaviour, IInteractableRpc, IHighlight
{
    public List<PowerCable> cables;
    public Outline outline;
    public AudioClip cutClip;
    public ParticleSystem sparkles;
    public LightDependentElement lightObject;

    public void ChangeHighlightThickness(float value)
    {
        
    }
    void Start()
    {
        lightObject.OnLightTurnOn();
    }

    public void DisableHighlight()
    {
        outline.enabled = false;
    }

    public void Highlight()
    {
        outline.enabled = true;
    }

    public void Interact(NetworkPlayerController owner)
    {
        Item _item = Inventory.Instance.GetMainItem(owner);
        if(_item != null && _item.itemName == ItemList.lvl4Pliers){
             CmdInteract();
        }
        else{
            UIManager.Instance.Message("pliers", "pliers_a");
        }
       
    }

    [Command (requiresAuthority = false)]
    void CmdInteract(){
        RpcInteract();
    }
    [ClientRpc]
    void RpcInteract(){
         foreach(PowerCable powerCable in cables){
            powerCable.OnCut();
        }
        AudioSource.PlayClipAtPoint(cutClip, transform.position, .3f);
        GetComponent<Collider>().enabled = false;
        sparkles.Play();
        lightObject.OnLightTurnOff();
    }
}
