using Mirror;
using UnityEngine;
using myTrs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Animations.Rigging;

public class Pentagram : NetworkBehaviour, IHighlight, IInteractableRpc
{   
    public Outline teddyBearHighlight;
    public Animation pentagramActivatedAnim;

    public Transform teddyPosition;
    public GameObject emptyTeddy;

    Coroutine PentagramActivationRoutine;
    public ParticleSystem activationParticle;
    public List<Animation> animatedOnActivation;
    public GameObject girl;

    [Space] [Header("rigs")]
    public List<Rig> girlRigs;
    public Animation rigsAnimation;
    public Transform keyHandParent;
    public Item _keyItem;
    float _currFogDensity;
    [SyncVar] Item _teddy;
    public AudioSource oneTimeSource;
    public AudioSource loopSource;
    public MeshRenderer mesh;

    public AudioClip start;
    public AudioClip end;
    public Animation pentLoopFade;
    public void ChangeHighlightThickness(float value)
    {
        
    }

    public void DisableHighlight()
    {
        teddyBearHighlight.enabled = false;
    }

    public void Highlight()
    {
        teddyBearHighlight.enabled = true;
    }

    public void Interact(NetworkPlayerController owner)
    {   Item _item;
        if(Inventory.Instance.GetSearchedItemOut(owner, out _item, ItemList.lvl4TeddyBear)){
            ActivatePentagram(_item);
        }
    }
    #region activation rpc
    public void ActivatePentagram(Item teddy){
        Inventory.Instance.ClearItem(Inventory.Instance.items.IndexOf(teddy), false);
        teddy.OnDropItem();
        CmdActivatePentagram(teddy);
    }

    [Command(requiresAuthority = false)]
    void CmdActivatePentagram(Item teddy){
        RpcActivatePentagram(teddy);
    }   

    [ClientRpc]
    void RpcActivatePentagram(Item teddy){
        
        gameObject.layer = 0;
        Transfrm.ResetPosition(teddy.transform.parent, teddyPosition, Vector3.zero);
        teddy.gameObject.layer = 0;
        pentagramActivatedAnim.Play();

        PentagramActivationRoutine = StartCoroutine(PentagramRoutine());
    }

    IEnumerator PentagramRoutine(){
       
        foreach(Animation anim in animatedOnActivation)
        {
            anim.Play();
        }
        oneTimeSource.PlayOneShot(start);
        yield return new WaitForSeconds(2f);
         activationParticle.Play();
        yield return new WaitForSeconds(5f);
        
        loopSource.Play();
        _currFogDensity = RenderSettings.fogDensity;
        RenderSettings.fogDensity = .0175f;
        RenderSettings.fogColor = new Color32(25,0,0,255);

        NetworkPlayerController.NetworkPlayer._cameraController.EnableRageEffect(.2f, 1f,2f);
         NetworkPlayerController.NetworkPlayer._cameraController.GlitchEffectVram(40,130,130);
         NetworkPlayerController.NetworkPlayer.EnterScaredMode(TimeRange.Short,ScaredModeProperty.ShockMode);

        emptyTeddy.SetActive(false);
        girl.SetActive(true);
        Transfrm.ResetPosition(_keyItem.transform.parent, keyHandParent, Vector3.zero);

        yield return new WaitForSeconds(4f);

        while(girlRigs[0].weight <1){
            girlRigs[0].weight += Time.deltaTime / 2f;
            yield return null;
        }


        yield return null;
    }
    #endregion

    public void OnKeyTakeEvent(){
        Debug.Log("key taken");
    StartCoroutine(OnKeyCoroutine());
    }

    IEnumerator OnKeyCoroutine(){
        yield return new WaitForSeconds(.5f);
        while(girlRigs[0].weight >0){
            girlRigs[0].weight -= Time.deltaTime;
            Debug.Log("loop");
            yield return null;
        }

        yield return new WaitForSeconds(3f);
        oneTimeSource.PlayOneShot(end);
        pentLoopFade.Play();
         NetworkPlayerController.NetworkPlayer._cameraController.EnableRageEffect(.2f, 1f,1f);
         NetworkPlayerController.NetworkPlayer._cameraController.GlitchEffectVram(40,130,130);
          NetworkPlayerController.NetworkPlayer._cameraController.GlitchEffectEnd(10,-25);

        yield return new WaitForSeconds(5f);
         NetworkPlayerController.NetworkPlayer._cameraController.DisableRageEffect();
        girl.SetActive(false);
        mesh.enabled = false;
        activationParticle.Stop();
        

        RenderSettings.fogDensity = _currFogDensity;
        RenderSettings.fogColor = Color.black;
        NetworkPlayerController.NetworkPlayer._cameraController.GlitchEffectEndReturn();
        yield return null;
    }

}
