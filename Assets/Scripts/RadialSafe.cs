using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadialSafe : FocusObject, IGenerable, IUnlockable
{
    [SerializeField] Transform rotatingElement;
    [SerializeField] AudioSource audioSource;

    [SerializeField] AudioClip rotateClip;
    [SerializeField] AudioClip confirmClip;
    [SerializeField] AudioClip unsealClip;

    [SerializeField] List<Renderer> quadRenderers;
    [SerializeField] List<Light> quadLights;

    [SerializeField] List<Material> materials;

    [SerializeField] Animation doorUnlockAnimation;
    [SerializeField] Collider _collider;

    IReceivePassword _iReceiveObject;
    [SerializeField] GameObject receivePasswordObject;

    int _currentNum = 0;
    int _currentIndex = 0;
    bool _isCoroutineOngoing;

    readonly public SyncList<int> combinationArray = new SyncList<int>();
    public int[] currentCombination;
    public int[] properCombination;

    [SyncVar] public string combinationString;

    [SerializeField] Collider _tempC;

    private void Start()
    {
        currentCombination = new int[5];
    }

    public void RotateElement()
    {
        if (_isCoroutineOngoing) return;
        AudioSource.PlayClipAtPoint(rotateClip, transform.position);

        rotatingElement.Rotate(0, 0, -36);

        _currentNum += 10;
        if (_currentNum > 90) _currentNum = 0;
    }

    public void ApplyCurrentNum()
    {
        if (_isCoroutineOngoing) return;

        AudioSource.PlayClipAtPoint(rotateClip, transform.position);
        StartCoroutine(PassRoutine());
    }

    public void ApplyGeneratedCode(string code)
    {
        combinationArray.Add(Random.Range(1,10) * 10);
        combinationArray.Add(Random.Range(1, 10) * 10);
        combinationArray.Add(Random.Range(1, 10) * 10);
        combinationArray.Add(Random.Range(1, 10) * 10);
        combinationArray.Add(Random.Range(1, 10) * 10);

        for(int a = 0; a < combinationArray.Count; a++)
        {
            combinationString += combinationArray[a];
        }

    }

    public void ShowGeneratedCode()
    {
        if(receivePasswordObject.TryGetComponent(out _iReceiveObject))
        {
            _iReceiveObject.DisplayPassword(combinationString);
        }

    }

    IEnumerator PassRoutine()
    {
        _isCoroutineOngoing = true;
        float _targetRatio = rotatingElement.transform.localEulerAngles.z > 0 ? 100 : -100;

        audioSource.Play();

        while (rotatingElement.transform.localEulerAngles.z > _targetRatio / _targetRatio)
        {
            rotatingElement.Rotate(0, 0, Time.deltaTime * _targetRatio);

            if (rotatingElement.transform.localEulerAngles.z - Time.deltaTime * _targetRatio > 0 && _targetRatio < 0
                ||
                rotatingElement.transform.localEulerAngles.z - Time.deltaTime * _targetRatio < 0 && _targetRatio > 0
                )
            {
                rotatingElement.localEulerAngles = new Vector3(0, 0, 0);
                break;
            }
            yield return null;
        }

        rotatingElement.localEulerAngles = new Vector3(0, 0, 0);
        audioSource.Stop();
        _isCoroutineOngoing = false;
        AudioSource.PlayClipAtPoint(confirmClip, transform.position);

        quadRenderers[_currentIndex].material = materials[1];
        quadLights[_currentIndex].color = Color.green;
        currentCombination[_currentIndex] = _currentNum;

        _currentNum = 0;

       
        if (_currentIndex == combinationArray.Count-1) 
        {
            if (Verify())
            {
                Unseal();
            }
            else
            {
                for (int a = 0; a < combinationArray.Count; a++)
                {
                    quadRenderers[a].material = materials[2];
                    quadLights[a].color = Color.red;
                }
            }
            _currentIndex = 0;
        }
        else
        {
            _currentIndex++;
        }

    }

    bool Verify()
    {
        for(int a = 0; a < combinationArray.Count-1; a++)
        {
            if (currentCombination[a] != combinationArray[a])
            {
                return false;
            }    
        }

        return true;
    }

    public void Unseal()
    {
        UnsealCmd();
    }

    [Command (requiresAuthority = false)]
    void UnsealCmd()
    {
        UnsealRpc();
    }
    [ClientRpc]
    void UnsealRpc()
    {
        doorUnlockAnimation.Play();
        _collider.enabled = false;
        focusObjectCollider.enabled = false;
        AudioSource.PlayClipAtPoint(unsealClip, transform.position);
        gameObject.layer = 0;
    }

    public bool Check()
    {
        throw new System.NotImplementedException();
    }

}
