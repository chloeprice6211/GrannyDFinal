using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;
using Cinemachine;
using Unity.Mathematics;
using UnityEngine.Rendering.PostProcessing;
using Unity.VisualScripting;

public class PlayerCamera : Mirror.NetworkBehaviour
{
    [SerializeField] Camera playerCamera;
    [SerializeField] GameObject lightSource;
    public CinemachineVirtualCamera virtualCamera;
    [SerializeField] NetworkPlayerController playerController;

    [SerializeField] Transform headAim;

    private CinemachineBasicMultiChannelPerlin perlin;

    [Header("Post process volume")]
    [SerializeField] PostProcessVolume postProcessVolume;
    public Grain _grain;
    public ColorGrading _colorGrading;
    public DepthOfField _depthOfField;
    private Vignette _vignette;
    [Range(0, 1)]
    public float grainMaxIntensity;

    

    public static PlayerControls controls;
    public bool canMove = true;

    Vector2 mouseLook;

    public float mouseSensitivity = 0.085f;
    float xRotation = 0f;

    public float rate = .2f;
    public float viewAngle = 20f;

    float _rageA;
    float _rageB;
    float _rageC;
    public ShaderEffect_VHS vhsEffect;
    public ShaderEffect_CorruptedVram vramEffect;
    public ShaderEffect_Tint tintEffect;
    public float vhsDistortion;
    public float vhsHeight;
    public float vhsWidth;


    public override void OnStartAuthority()
    {
        playerCamera.gameObject.SetActive(true);
        virtualCamera.gameObject.SetActive(true);

        postProcessVolume.profile = GameManager.Instance.volume;

        postProcessVolume.profile.TryGetSettings(out _grain);
        postProcessVolume.profile.TryGetSettings<ColorGrading>(out _colorGrading);
        postProcessVolume.profile.TryGetSettings(out _vignette);
        postProcessVolume.profile.TryGetSettings(out _depthOfField);

        _grain.intensity.value = GameManager.Instance.defaultVolume.GetSetting<Grain>().intensity.value;
        _grain.size.value = GameManager.Instance.defaultVolume.GetSetting<Grain>().size.value;
        _grain.lumContrib.value = GameManager.Instance.defaultVolume.GetSetting<Grain>().lumContrib.value;

        _vignette.smoothness.value = GameManager.Instance.defaultVolume.GetSetting<Vignette>().smoothness.value;
        _vignette.roundness.value = GameManager.Instance.defaultVolume.GetSetting<Vignette>().roundness.value;
        _vignette.intensity.value = GameManager.Instance.defaultVolume.GetSetting<Vignette>().intensity.value;

        _colorGrading.postExposure.value = GameManager.Instance.defaultVolume.GetSetting<ColorGrading>().postExposure.value;
        _colorGrading.mixerRedOutRedIn.value = GameManager.Instance.defaultVolume.GetSetting<ColorGrading>().mixerRedOutRedIn.value;
        _colorGrading.saturation.value = GameManager.Instance.defaultVolume.GetSetting<ColorGrading>().saturation.value;
        _colorGrading.colorFilter.value = GameManager.Instance.defaultVolume.GetSetting<ColorGrading>().colorFilter.value;

        _depthOfField.focalLength.value = GameManager.Instance.defaultVolume.GetSetting<DepthOfField>().focalLength.value;

        mouseSensitivity = PrefsSettings.s_maxSens * PrefsSettings.s_sens;

        _colorGrading.postExposure.value = GameManager.Instance.defaultVolume.GetSetting<ColorGrading>().postExposure.value + (PrefsSettings.s_postExposure * 2);
        perlin = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        virtualCamera.m_Lens.FieldOfView = PrefsSettings.s_fov;

        controls = new PlayerControls();
        controls.Player.Look.Enable();

    }
    private void Start()
    {
        //vhsEffect = playerCamera.AddComponent<ShaderEffect_VHS>();
        //tintEffect = playerCamera.AddComponent<ShaderEffect_Tint>();
        //vramEffect = playerCamera.AddComponent<ShaderEffect_CorruptedVram>();

    }
    private void OnDisable()
    {
        controls.Player.Look.Disable();
    }
    void LateUpdate()
    {
        if (hasAuthority)
        {
            InventorySway();
        }

        mouseLook = controls.Player.Look.ReadValue<Vector2>() * mouseSensitivity;

        if (!canMove) return;

        xRotation -= mouseLook.y;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        if (hasAuthority)
        {
            virtualCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            transform.Rotate(Vector3.up * mouseLook.x);
        }
        
        lightSource.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        lightSource.transform.Rotate(Vector3.up, mouseLook.x);

        headAim.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        headAim.Rotate(Vector3.up, mouseLook.x);



    }
    void InventorySway()
    {
        float xSway = mouseLook.x;
        float ySway = mouseLook.y;

        if (!canMove)
        {
            xSway = 0;
            ySway = 0;
        }

        Quaternion xAdjustment = Quaternion.AngleAxis(-4 * xSway, Vector3.up);
        Quaternion yAdjustment = Quaternion.AngleAxis(4 * ySway, Vector3.right);

        Quaternion targetRotation = Quaternion.Euler(0, 180, 0) * xAdjustment * yAdjustment;

        playerController.inventory.localRotation = Quaternion.Lerp(playerController.inventory.localRotation, targetRotation, Time.deltaTime * 10);
        playerController.flashlightPosition.localRotation = Quaternion.Lerp(playerController.inventory.localRotation, targetRotation, Time.deltaTime * 10);
    }

    bool isRaged;

    #region Device camera positioning

    public void CenterCamera()
    {
        if ((int)virtualCamera.transform.localEulerAngles.x == (int)viewAngle) return;

        canMove = false;

        float angle = ConvertEulerAngle(virtualCamera.transform.localEulerAngles.x);

        if (angle > viewAngle)
        {
            StartCoroutine(CenterCameraFromBottomCoroutine(angle));
        }
        else
        {
            StartCoroutine(CenterCameraFromTopCoroutine(angle));
        }
    }

    public IEnumerator CenterCameraFromBottomCoroutine(float angle)
    {
        float _temp;

        while (angle >= viewAngle)
        {
            _temp = Time.deltaTime * 50;

            virtualCamera.transform.Rotate(-_temp, 0, 0);
            angle -= _temp;

            //virtualCamera.transform.Rotate(-rate, 0, 0);
            //angle -= rate;

            yield return null;
        }

        ResetCameraPosition(viewAngle);

    }

    public IEnumerator CenterCameraFromTopCoroutine(float angle)
    {
        float _temp;

        while (angle <= viewAngle)
        {
            _temp = Time.deltaTime * 50;

            virtualCamera.transform.Rotate(_temp, 0, 0);
            angle += _temp;

            //virtualCamera.transform.Rotate(rate, 0, 0);
            //angle += rate;

            yield return null;
        }

        ResetCameraPosition(viewAngle);

    }


    private float ConvertEulerAngle(float angle)
    {
        return (angle > 180) ? angle - 360 : angle;
    }

    public void ResetCameraPosition(float angle)
    {
        xRotation = angle;
        mouseLook = new Vector2(0, 0);
        virtualCamera.transform.localRotation = Quaternion.Euler(angle, 0, 0);

    }

    #endregion

    #region Screen effects

    public IEnumerator EnableScaredScreenEffect(ScaredModeProperty mode)
    {
        float _effectGainRate;
        float _grainMaxIntensity;

        switch (mode._mode)
        {
            case ScaredMode.Shock:
                _effectGainRate = 0.02f * 3;
                _grainMaxIntensity = grainMaxIntensity * 1.5f;
                playerController.hasBeenShocked = true;
                break;

            case ScaredMode.Fear:
                _effectGainRate = 0.02f;
                _grainMaxIntensity = grainMaxIntensity;
                break;

            default:
                _effectGainRate = 0.02f;
                _grainMaxIntensity = grainMaxIntensity;
                break;
        }

        while (_grain.intensity.value <= _grainMaxIntensity)
        {
            _grain.intensity.value += _effectGainRate / 4;
            _grain.size.value += _effectGainRate / 3;

            if (_grain.intensity.value >= 1) break;

            _colorGrading.saturation.value -= _effectGainRate * 10;

            if (playerController.hasBeenShocked)
            {
                if (_vignette.intensity.value <= .7f)
                {
                    _vignette.intensity.value += _effectGainRate / 20f;
                }

            }

            yield return new WaitForSeconds(.05f);
        }
    }

    public IEnumerator DisableScaredScreenEffect()
    {
        StopCoroutine("EnableScaredScreefEffect");

        float _effectGainRate = 0.02f;

        while (_grain.intensity.value >= 0)
        {
            _grain.intensity.value -= _effectGainRate / 3.5f;
            
            if(_grain.size.value >= 0)
            {
                _grain.size.value -= _effectGainRate;
            } 

            if(_colorGrading.saturation.value <= 50)
            {
                _colorGrading.saturation.value += _effectGainRate * 35;
            }
            

            if (playerController.hasBeenShocked)
            {
                if (_vignette.intensity.value >= 0.55)
                {
                    _vignette.intensity.value -= _effectGainRate /3;
                }
            }

            yield return new WaitForSeconds(.05f);
        }

        Debug.Log("STOPPED");

        playerController.hasBeenShocked = false;
        _grain.intensity.value = 0;

    }

    public void ReturnNormalVisionAfterDeath()
    {
        StopAllCoroutines();

        _grain.intensity.value = 0;
        _grain.size.value = .3f;

        _colorGrading.postExposure.value = GameManager.Instance.defaultVolume.GetSetting<ColorGrading>().postExposure.value;

        _vignette.smoothness.value = GameManager.Instance.defaultVolume.GetSetting<Vignette>().smoothness.value;
        _vignette.roundness.value = GameManager.Instance.defaultVolume.GetSetting<Vignette>().roundness.value;
        _vignette.intensity.value = GameManager.Instance.defaultVolume.GetSetting<Vignette>().intensity.value;

        _colorGrading.mixerRedOutRedIn.value = GameManager.Instance.defaultVolume.GetSetting<ColorGrading>().mixerRedOutRedIn.value;
        _colorGrading.saturation.value = GameManager.Instance.defaultVolume.GetSetting<ColorGrading>().saturation.value;
        StartCoroutine(EndShakingCameraRoutine(.1f));
    }

    public void EnableDeathScreenEffect()
    {
        StopAllCoroutines();
        StartCoroutine(DeathScreenEffectRoutine());
    }

    public void EnablePreDeathScreenEffect()
    {
        StopAllCoroutines();
        StartCoroutine(PreDeathScreenEffectRoutine());
    }

    public void DisableDeathScreenEffect()
    {
        _colorGrading.saturation.value = 0;
        _colorGrading.postExposure.value = 0;
        _colorGrading.mixerBlueOutRedIn.value = 100;
        _vignette.intensity.value = .7f;
        _grain.intensity.value = 1.5f;

        StopCoroutine("DeathScreenEffectRoutine");
    }

    private IEnumerator DeathScreenEffectRoutine()
    {
        UIManager.Instance.preDeathGlitchImage.enabled = false;
        _colorGrading.saturation.value = 0;
        _colorGrading.mixerRedOutRedIn.value = 200;

        yield return new WaitForSeconds(1f);

        while (_vignette.intensity.value < 1)
        {
            _vignette.intensity.value += .025f;
            _colorGrading.postExposure.value -= .8f;

            yield return new WaitForSeconds(.15f);
        }
    }

    private IEnumerator PreDeathScreenEffectRoutine()
    {
        //_colorGrading.saturation.value = 0;
        //_colorGrading.mixerRedOutRedIn.value = 200;

        //yield return new WaitForSeconds(1f);
        perlin.m_AmplitudeGain = .27f;
        //_colorGrading.postExposure.value = GameManager.Instance.defaultVolume.GetSetting<ColorGrading>().postExposure.value;
        if(isRaged) DisableRageEffect();
        
        UIManager.Instance.preDeathGlitchImage.enabled = true;
        UIManager.Instance.preDeathGlitchImage.color = new Color(1, 1, 1, 0);

        while (UIManager.Instance.preDeathGlitchImage.color.a < .35f)
        {
            _colorGrading.saturation.value -= 6;


            if (_vignette.intensity.value < .6f)
            {
                _vignette.intensity.value += .006f;
            }
            else if(_vignette.intensity.value >= .6f)
            {
                _vignette.intensity.value -= .006f;
            }

            _colorGrading.postExposure.value -= .05f;
            UIManager.Instance.preDeathGlitchImage.color = new Color(
                1,
                1,
                1,
                UIManager.Instance.preDeathGlitchImage.color.a + 0.01f);

            _grain.size.value += .04f;
            _grain.intensity.value += .04f;

            yield return new WaitForSeconds(.05f);
        }
    }

    #endregion

    #region Camera shaking

    public void ShakeCamera(float time, float intensity, float rate)
    {
        if (!hasAuthority) return;

        StartCoroutine(ShakeCameraRoutine(time, intensity, rate));
    }

    public void EndCameraShaking(float rate)
    {
        StopCoroutine("ShakeCameraRoutine");
        StartCoroutine(EndShakingCameraRoutine(rate));
    }

    /// <summary>
    /// Cinemachine camera shaking
    /// </summary>
    /// <param name="time">Time till intensity ends. Pass 0 for endless intensity</param>
    /// <param name="intensity">Shaking intensity</param>
    /// <returns></returns>
    private IEnumerator ShakeCameraRoutine(float time, float intensity, float rate)
    {
        float _currentAmplitude = perlin.m_AmplitudeGain;

        while (_currentAmplitude <= intensity)
        {
            _currentAmplitude += rate;
            perlin.m_AmplitudeGain = _currentAmplitude;

            yield return new WaitForSeconds(.05f);
        }

        if (time != 0)
        {
            yield return new WaitForSeconds(time);
            StartCoroutine(EndShakingCameraRoutine(rate / 2));
        }
    }

    private IEnumerator EndShakingCameraRoutine(float rate)
    {
        float _currentAmplitude = perlin.m_AmplitudeGain;

        while (_currentAmplitude >= 0)
        {
            _currentAmplitude -= rate;
            perlin.m_AmplitudeGain = _currentAmplitude;

            yield return new WaitForSeconds(.1f);
        }

        perlin.m_AmplitudeGain = 0;

    }

    #endregion

    public void DisableCameraControls()
    {
        controls.Disable();
        canMove = false;
    }

    public void EnableCameraControls()
    {
        controls.Enable();
        canMove = true;
    }

    #region rage effects
    public void EnableRageEffect(float _a = .2f, float _b = 1.5f, float _c = 1f){

        _rageA = _a;
        _rageB = _b;
        _rageC = _c;

        isRaged = true;
        StartCoroutine(RageScreenStartRoutine());
    }
    public void DisableRageEffect(){
        isRaged = false;
        StartCoroutine(RageScreenEndRoutine());
    }

    IEnumerator RageScreenStartRoutine(){

        float _depthInitiateValueSeconds = 0.05f;
        ShakeCamera(_rageA,_rageB, _rageC);
        _colorGrading.colorFilter.value = Color.red;
        Debug.Log(_colorGrading.colorFilter.value  + "RED VALUE");

        while(_depthOfField.focalLength<20){
            _depthOfField.focalLength.value+=3f;
            yield return new WaitForSeconds(_depthInitiateValueSeconds);
        }
        
         while(_depthOfField.focalLength>1){
            _depthOfField.focalLength.value-=.33f;
            yield return new WaitForSeconds(_depthInitiateValueSeconds);
        }
        

        //_colorGrading.mixerRedOutRedIn.value = 200f;

        yield return new WaitForSeconds(1.5f);
    }
    IEnumerator RageScreenEndRoutine(){
        float _r = 0;
        while(_colorGrading.colorFilter.value.b <=1){
            _colorGrading.colorFilter.value = new Color(1,_r,_r);
            _r+= 0.01f;
            yield return new WaitForSeconds(0.033f);
        }
        _colorGrading.colorFilter.value = Color.white;
        _depthOfField.focalLength.value = 1;
        yield return new WaitForSeconds(0);
    }
    #endregion

    #region glitchEffects

    [QFSW.QC.Command ("g")]
    public void GlitchEffectVram(float a, float b, float c){
        StartCoroutine(GlitchVramRoutine());
        ShakeCamera(.2f,2f,2f);
        vhsDistortion = a;
        vhsHeight = b;
        vhsWidth = c;
    }

    public void GlitchEffectEnd(float a, float b){
        StartCoroutine(GlitchNewRoutine(a,b));
    }

    public void GlitchEffectEndReturn(){
        StartCoroutine(GlitchEffectReturn());
    }

    IEnumerator GlitchVramRoutine(){
        vhsEffect.enabled = true;
        vramEffect.enabled = true;

         
        while(vhsEffect.distortion <=50){
            vramEffect.shift += Time.deltaTime * 10;
            vhsEffect.distortion +=Time.deltaTime * vhsDistortion;
            vhsEffect.height += Time.deltaTime * vhsHeight;
            vhsEffect.width += Time.deltaTime * vhsWidth;
            yield return null;
        }

         while(vhsEffect.distortion >= 0){
            vhsEffect.distortion -=Time.deltaTime * vhsDistortion;
            vhsEffect.height -= Time.deltaTime * vhsHeight;
            vhsEffect.width -= Time.deltaTime * vhsWidth;
            vramEffect.shift -= Time.deltaTime * 7;
            yield return null;
        }

        while( vramEffect.shift >0){
             vramEffect.shift -= Time.deltaTime * 5;
             yield return null;
        }
        vramEffect.shift = 0;

        vhsEffect.distortion = 0f;
        vhsEffect.height = 0f;
        vhsEffect.width = 0f;

        vramEffect.enabled = false;
         vhsEffect.enabled = false;
        
    }
    IEnumerator GlitchNewRoutine(float tintV, float tintY){
        tintEffect.enabled = true;

          while(tintEffect.v <= tintV){
            tintEffect.v += Time.deltaTime *tintV;
           
            yield return null;
        }

        while(tintEffect.y >= tintY){
            tintEffect.y -= Time.deltaTime *(-tintY);
           
            yield return null;
        }

        yield return null;
    }

    IEnumerator GlitchEffectReturn(){

        tintEffect.v = 1;
        //tintEffect.y = 1;

          while(tintEffect.y <1){
             tintEffect.y += Time.deltaTime *3.5f;
             yield return null;
     }
        tintEffect.enabled = false;
        yield return null;
    }
    #endregion
}
