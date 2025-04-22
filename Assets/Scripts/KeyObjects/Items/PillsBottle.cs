using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PillsBottle : Item, ITakeable, IHasControls
{
    [SerializeField] AudioClip takePillSound;
    [SerializeField] Animation _animation;
    [SerializeField] Canvas controlsCanvas;

    public int pillsAmount = 3;
    private PlayerControls pillBottleControls;
    private InputAction _takePillAction;


    bool _hasControlsMessageAppeared;

    
    public override void Awake()
    {
        base.Awake();
        pillBottleControls = new();
        pillBottleControls.PillBottle.TakePill.performed += TakePill;

        _takePillAction = pillBottleControls.PillBottle.TakePill;

        pillsAmount = SetupPanel.LevelSettings.PillsAmount;
    }


    public override void TakeItem(NetworkPlayerController owner)
    {
        base.TakeItem(owner);

        if (owner.hasAuthority)
        {
            controlsCanvas.enabled = true;
            ReturnControls();
        }
       
    }

    public override void OnDropItem()
    {
        if (_animation.isPlaying)
        {
            _animation.Stop();
        }

        base.OnDropItem();
        controlsCanvas.enabled = false;

        DisableItemControls();
    }

    public void TakePill(InputAction.CallbackContext context)
    {
        if(pillsAmount == 0)
        {
            
            UIManager.Instance.Message("pillsbottle", "noPills_A");
            return;
        }

        CmdTakePill();
        _owner.EnablePillEffect();

    }

    [Command (requiresAuthority = false)]
    void CmdTakePill()
    {
        RpcTakePill();
    }
    [ClientRpc]
    void RpcTakePill()
    {
        _animation.Play();
        AudioSource.PlayClipAtPoint(takePillSound, transform.position, .5f);
        pillsAmount--;
    }

    public override void Inspect()
    {
        //controlsCanvas.enabled = false;
        //_takePillAction.Disable();
        //base.Inspect();
    }

    public override void ReturnFromInspect()
    {
        //controlsCanvas.enabled = true;
        //_takePillAction.Enable();
        //base.ReturnFromInspect();
    }

    public void DisableItemControls()
    {
        _takePillAction.Disable();
    }

    public void ReturnControls()
    {
        _takePillAction.Enable();
    }

}
