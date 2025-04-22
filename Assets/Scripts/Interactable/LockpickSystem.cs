using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class LockpickSystem : FocusObject
{
    public List<Animation> teethAnims;
    KeyControl keyControl;
    InputActionMap lockpickActionMap;
    PlayerControls _controls;

    Coroutine lockpickRoutine;
    Coroutine releaseDelayRoutine;

    public Door lockpickedDoor;
    bool _isSucceed;
    [SyncVar] public bool hasLockpickBeenInstalled;

    char[] myarray = new char[]{'A','D','W'};
    float _timer = 0f;
    float _counter;
    public GameObject lockPickUI;
    int successedTeeth;
    public Animation holderAnimation;
    public Animation submitButtonAnimation;
    public Animation lockPickingAnimation;

    public TextMeshProUGUI lockpickSubmitButtonChar;

    public void InitializeLockpick(){
        lockpickRoutine = StartCoroutine(LockpickRoutine());
        Debug.Log("well this is mine");
    }

    public override void Interact(NetworkPlayerController owner)
    {     
         if(!lockPickingAnimation.gameObject.activeInHierarchy) CmdInteract();
    
    RiseAllTeath();
        lockpickedDoor.gameObject.layer = 0;
     holderAnimation.Play("teethHolderWhite");
    lockPickUI.SetActive(true);
        base.Interact(owner);
        InitializeLockpick();
    }

    public override void Release()
    {  
        submitButtonAnimation.gameObject.SetActive(false);
        lockpickedDoor.gameObject.layer = 7;
         lockPickUI.SetActive(false);
         StopCoroutine(lockpickRoutine);
         if(releaseDelayRoutine!=null) StopCoroutine(releaseDelayRoutine);
         lockPickingAnimation["LockPicking"].speed = 0f;
        base.Release();
    }

    void RiseAllTeath(){
        foreach(Animation teethAnim in teethAnims)
        {
            teethAnim.Play("lockpickTeethRenew");
        }

    }

    IEnumerator LockpickRoutine(){
        
        

        successedTeeth = 0;

       yield return new WaitForSeconds(1.75f);
       submitButtonAnimation.gameObject.SetActive(true);
       if(!lockPickingAnimation.isPlaying) lockPickingAnimation.Play("LockPicking");
       
        for(int a = 0;a<teethAnims.Count;a++){
             char currentChar = myarray[UnityEngine.Random.Range(0, 3)];
             submitButtonAnimation.gameObject.SetActive(true);
            Debug.Log("current char is " + currentChar);

            lockpickSubmitButtonChar.text = currentChar.ToString();
            submitButtonAnimation.Play("lockpickButtonIdle");

            if(a == 0) _timer = 2f;
            else _timer = 1f;

            while(true){
                _counter+=Time.deltaTime;
                if( lockPickingAnimation["LockPicking"].speed<=1){
                    lockPickingAnimation["LockPicking"].speed += Time.deltaTime;
                }
                if(Keyboard.current.aKey.isPressed && currentChar == 'A'){
                    submitButtonAnimation.Play("lockpickButtonSuccess");
                     teethAnims[a].Play("lockpickTeeth");
                    successedTeeth++;
                    Debug.Log("success, a is pressed");
                    break;
                }
                else if(Keyboard.current.dKey.isPressed && currentChar == 'D'){
                    submitButtonAnimation.Play("lockpickButtonSuccess");
                     teethAnims[a].Play("lockpickTeeth");
                    successedTeeth++;
                    Debug.Log("success, d is pressed");
                    break;
                }
                else if(Keyboard.current.wKey.isPressed && currentChar == 'W'){
                    submitButtonAnimation.Play("lockpickButtonSuccess");
                     teethAnims[a].Play("lockpickTeeth");
                    successedTeeth++;
                    Debug.Log("success, w is pressed");
                    break;
                }

                if(Keyboard.current.aKey.isPressed && currentChar != 'a'){
                    FailQTE();
                    break;
                }
                else if(Keyboard.current.wKey.isPressed && currentChar != 'w'){
                    
                    FailQTE();
                     break;
                }
                else if(Keyboard.current.dKey.isPressed && currentChar != 'd'){
                    FailQTE();
                     break;
                }

                if(_counter >= _timer){
                    FailQTE();
                    break;
                }

                   yield return null;
            }
            
           
            _counter = 0f;
            yield return new WaitForSeconds(0.75f);
             
            
        }
        Debug.Log("all succeded");

         holderAnimation.Play("teethHolderSuccess");
         lockpickedDoor.UnlockDoorWOKeyCmd();
         lockPickingAnimation["LockPicking"].speed = 0f;
         GetComponent<Collider>().enabled = false;
    }

    void FailQTE(){
        lockPickingAnimation["LockPicking"].speed = 0f;
        submitButtonAnimation.Play("lockpickButtonFail");
        StopCoroutine(lockpickRoutine);
        releaseDelayRoutine=  StartCoroutine(FailDelayedCoroutine());  
    }

    IEnumerator FailDelayedCoroutine(){
       
         for(int a = 0;a<successedTeeth;a++){
            teethAnims[a].Play("lockpickTeethRenew");
        }
        holderAnimation.Play();
        Debug.Log("failed");
         yield return new WaitForSeconds(1.5f);

        Release();
        
    }

    [QFSW.QC.Command ("comm1")]
    void Test(){
        Interact(NetworkPlayerController.NetworkPlayer);
    }

    [Command (requiresAuthority = false)]
    void CmdInteract(){
        hasLockpickBeenInstalled = true;
        RpcInteract();
    }
    [ClientRpc]
    void RpcInteract(){
    {
        lockPickingAnimation.gameObject.SetActive(true);
    } 
    }
}