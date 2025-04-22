using System.Collections;
using System.Collections.Generic;
using Mirror;
using Mono.CSharp;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PhoneTransciever : NetworkBehaviour, IInteractableRpc
{
    public List<Animation> fuseAnimation;
    public List<Transform> fusePosition;

    [Space] [Header("ui")]
    public TextMeshProUGUI progressBarText;
    public TextMeshProUGUI connectedStatusText;
    public TextMeshProUGUI remainedText;
    public TextMeshProUGUI awaitingText;
    public Image imageConnectingBar;

    [SyncVar] int currentFuse = 0;
    Fuse _fuse;
     Item _item;

    #region insert
    
    public void InsertFuse(Item fuse){
        CmdInsertFuse(fuse);
    }

    public void Interact(NetworkPlayerController owner)
    {   
       
        if(Inventory.Instance.GetSearchedItemOut(owner, out _item, ItemList.lvl4Fuse )){
            _fuse = _item as Fuse;
            Inventory.Instance.ClearItem(Inventory.Instance.items.IndexOf(_fuse), false);
            _fuse.OnDropItem();
            InsertFuse(_fuse);
            

        }
        
       
    }

    [Command (requiresAuthority = false)]
    void CmdInsertFuse(Item fuse){
        RpcInsertFuse(fuse);
    }

    [ClientRpc]
    void RpcInsertFuse(Item fuse){
        Debug.Log(fuse.gameObject.name + " inserted");

        (fuse as Fuse).tubeAnimation.Play();

        fuse.transform.parent.SetParent(fusePosition[currentFuse]);
         fuse.transform.parent.localPosition = Vector3.zero;
        fuse.transform.parent.localRotation = Quaternion.Euler(-90,0,0);

        fuse.gameObject.layer = 0;
        //fuse.GetComponent<Collider>().enabled = false;
        

        fuseAnimation[currentFuse].Play();

        currentFuse++;

        remainedText.text = currentFuse.ToString() + "/4 installed";



        if(currentFuse == 4){
            Debug.Log("completed");
            connectedStatusText.text = "[CONNECTING]";
            connectedStatusText.color = Color.black;
            StartCoroutine(ConnectingRoutine());
            remainedText.enabled = false;
            gameObject.layer = 0;
        }
    }
    #endregion

    IEnumerator ConnectingRoutine(){
        while(imageConnectingBar.fillAmount <= .99){
            imageConnectingBar.fillAmount += Time.deltaTime/100 * 2;
            progressBarText.text = (imageConnectingBar.fillAmount * 100).ToString("0.0") + "%";
            yield return null;
        }

        awaitingText.text = "CONNECTED!";
        imageConnectingBar.enabled = false;
        progressBarText.enabled = false;
        connectedStatusText.enabled = false;
        yield return null;
    }
}
