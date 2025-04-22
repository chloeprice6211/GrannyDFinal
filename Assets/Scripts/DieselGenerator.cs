using System;
using Mirror;
using TMPro;
using UnityEngine;

public class DieselGenerator : FocusObject, ICustomGenerable, IRefualable
{   
    [Space] [Header("ui")]
    public TextMeshProUGUI voltageTextValue;
    public TextMeshProUGUI switchTextValue;
    public TextMeshProUGUI keyTextValue;
    public TextMeshProUGUI mainSwTextValue;
    public TextMeshProUGUI yellowSwitchValue;


    [Space] [Header("sounds")]
    public AudioClip successKeyClip;
    public AudioSource generatorSource;
    public AudioClip switchClip;
    public AudioClip voltageKeysClip;
    public AudioClip buttonClip;


    [Space] [Header("sync vars")]
    [SyncVar] int voltageValue = 200;
    [SyncVar] bool isGeneratorOn;
    [SyncVar] bool switchValue;
    [SyncVar] float mainSwValue;
    bool isSyncCompleted = true;
    [SyncVar] bool isKey;
    [SyncVar] bool isButtonActivated;
    float fuelLevel;

    [Space] [Header("correct parameters")]

    [SyncVar] int voltageValueC;
    [SyncVar] bool switchValueC;
    [SyncVar] float mainSwValueC;
    [SyncVar] bool isButtonActivatedC;


    [Space] [Header("misc")]
    public Animation switchAnimation;
    public Transform keyInstallPosition;
    public Animation keyHoleAnimation;
    public Animation buttonAnimation;

    Vector3 keyScale = new Vector3(0.0210699998f,0.0210699998f,0.0210699998f);
    Key _key;
    NetworkPlayerController _owner;
    public GameObject mainSWRotatedElement;
    public Renderer _buttonRenderer;
    public Material buttonActive;
    public Material buttonInvactive;
    public  Light buttonLightSource;
    public TextMeshProUGUI onText;
    public LightDependentElement lightElement;
    public FuelTankNew tank;

    
    void Start()
    {
        
    }

    public override void Interact(NetworkPlayerController owner)
    {
        base.Interact(owner);

        if(Inventory.Instance.HasAnyItem(owner)){
            Item currItem = Inventory.Instance.GetMainItem(owner);

            if(currItem is Key && (currItem as Key).objectiveType == ObjectiveType.LVL4Generator){
                currItem.OnItemUsed(owner);
                KeyInsert(currItem as Key);
               
            }
        }

    }

    public void OnMainSwitchClick(){
        OnMainSw();
    }

    #region onVoltChange
    public void OnVoltChange(int value){
        if(!isSyncCompleted) return;
        int currentValue = Int32.Parse( voltageTextValue.text);

        if(currentValue <=200 && value <0){
            currentValue = 450;
        }
        else if(currentValue >=450 && value > 0){
            currentValue = 200;
        }
        else {
            currentValue += value;
        }
        Debug.Log(currentValue + " current voltage");

        OnVoltChangeCommand(currentValue);

    }

    [Command (requiresAuthority = false)]
    void OnVoltChangeCommand(int volt){
        voltageValue = volt;
        OnVoltChangeRpc();
    }

    [ClientRpc]
    void OnVoltChangeRpc(){
         voltageTextValue.text = voltageValue.ToString();
         AudioSource.PlayClipAtPoint(voltageKeysClip, transform.position, .5f);
        isSyncCompleted = true;
    }

    #endregion
    
    #region onSwitchChange

    public void OnSwitchValueChange(){
        if(!isSyncCompleted || switchAnimation.isPlaying) return;

        OnSwitchValueChangeCommand(!switchValue);
    }

    [Command (requiresAuthority = false)]
    void OnSwitchValueChangeCommand(bool value){
        switchValue = value;
        OnSwitchChangedRpc();
    }

    [ClientRpc]
    void OnSwitchChangedRpc(){
        isSyncCompleted = true;
        switchTextValue.text = switchValue.ToString();

        AudioSource.PlayClipAtPoint(switchClip, transform.position, .5f);

        if(switchValue){
            switchAnimation.Play("FuseboxSwitch");
        }
        else{
            switchAnimation.Play("FuseboxSwitchOff");
        }

    }

    #endregion

    #region onKeyInsert

    public void KeyInsert(Key key){
        CmdKeyInsert(key);
    }

    [Command (requiresAuthority = false)]
    void CmdKeyInsert(Key key){
        isKey = true;

        RpcKeyInsert(key);
    }

    [ClientRpc]
    void RpcKeyInsert(Key key){

        AudioSource.PlayClipAtPoint(successKeyClip, transform.position, .7f);

        Transform itemParent = key.transform.parent;

        itemParent.SetParent(keyInstallPosition);
        itemParent.localPosition = Vector3.zero;
        itemParent.localRotation = Quaternion.identity;

        key.GetComponent<Collider>().enabled = false;
        keyHoleAnimation.Play();

        itemParent.transform.localScale = keyScale;

        keyTextValue.text = "TRUE";
    }


    #endregion
    
    #region onMainSW

    public void OnMainSw(){
        if(!isSyncCompleted) return;
        isSyncCompleted = false;

        CmdOnMainSw();



    }

    [Command (requiresAuthority = false)]
    void CmdOnMainSw(){
        mainSwValue +=45;
        if(mainSwValue >=360){
            mainSwValue = 0;
        }
        RpcOnMainSw();
    }

    [ClientRpc]
    void RpcOnMainSw(){
        isSyncCompleted = true;

        mainSwTextValue.text = "T_" + (mainSwValue+1/10).ToString("0.0");

        AudioSource.PlayClipAtPoint(switchClip, transform.position, .5f);
        mainSWRotatedElement.transform.Rotate(45,0,0);

        Debug.Log("current sw is " + mainSwValue);
    }

    #endregion 

    #region onPanel

    #region OnStart

    public void OnStartButton(){
        if(Check() && !isGeneratorOn){
            CmdStartGenerator();
        }
        else if(isGeneratorOn){
            CmdStopGenerator();
        }
    }

    bool Check(){
        if(
            voltageValueC == voltageValue &&
            switchValueC == switchValue &&
            mainSwValueC == mainSwValue &&
            isButtonActivatedC == isButtonActivated &&
            isKey == true &&
            tank.fuelLevel >= tank.maxCapacity
        ) return true;

        return false;
    }

    [Command (requiresAuthority = false)]
    void CmdStartGenerator(){
        isGeneratorOn = true;
        lightElement.isPowered = true;
        RpcStartGenerator();
    }
    [ClientRpc]
    void RpcStartGenerator(){
        generatorSource.Play();
        Debug.Log("generator started");
        onText.enabled = true;
    }
    
    [Command (requiresAuthority = false)]
    void CmdStopGenerator(){
        isGeneratorOn = false;
        lightElement.isPowered = false;
        RpcStopGenerator();
    }
    [ClientRpc]
    void RpcStopGenerator(){
        generatorSource.Stop();
        Debug.Log("generator stopped");
        onText.enabled = false;
    }

    #endregion

    public void OnPanelButton(){
        ActivateCmd();
    }

     [Command (requiresAuthority = false)]
    void ActivateCmd()
    {
        isButtonActivated = !isButtonActivated;
        ActivateRpc();
    }
    [ClientRpc]
    void ActivateRpc()
    {
        AudioSource.PlayClipAtPoint(buttonClip, transform.position);

        if (isButtonActivated)
        {
            buttonAnimation.Play("ButtonPressSwitch");
            _buttonRenderer.material = buttonActive;
            yellowSwitchValue.text = "GW";
        }
        else
        {
            buttonAnimation.Play("ButtonReturnSwitch");
            _buttonRenderer.material = buttonInvactive;
            yellowSwitchValue.text = "TW";
        }
    }

    #endregion

    [QFSW.QC.Command("generate")]
    public void CustomGenerate(){
        voltageValueC = UnityEngine.Random.Range(5, 10) * 50;
        switchValueC = UnityEngine.Random.Range(0,2) == 1 ? true : false;
        mainSwValueC = UnityEngine.Random.Range(2, 7) * 45;
        isButtonActivatedC = UnityEngine.Random.Range(0,2) == 1 ? true : false;
    }

    public void Refill()
    {
        tank.fuelLevel = tank.maxCapacity;
    }
}
