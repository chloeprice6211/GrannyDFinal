using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombinationLock : FocusObject, IGenerable
{
    [SyncVar] public string combinationToUnlock;
    [SyncVar] public string currentCombination;
    [SerializeField] GameObject objectToUnlock;

    [SerializeField] AudioClip combinationChangeSound;
    [SerializeField] Animation unlockAnimation;

    IUnlockable iUnlockableObject;
    IReceivePassword _iReceivePassword;

    [SerializeField] GameObject objectToReceive;

    public List<PadlockGear> gears;

    public void Verify(int gearIndex)
    {
        AudioSource.PlayClipAtPoint(combinationChangeSound, transform.position);

        currentCombination = string.Empty;

        foreach (PadlockGear gear in gears)
        {
            currentCombination += gear.currentChar.ToString();
        }

        if (combinationToUnlock == currentCombination)
        {
            UnsealCmd();

        }
    }

    public void ApplyGeneratedCode(string code)
    {
        for (int a = 0; a < gears.Count; a++)
        {
            combinationToUnlock += Random.Range(1, 10).ToString();
        }
    }

    public void ShowGeneratedCode()
    {
        if (objectToReceive.TryGetComponent(out _iReceivePassword))
            _iReceivePassword.DisplayPassword(combinationToUnlock);
    }

    [Command(requiresAuthority = false)]
    void UnsealCmd()
    {
        UnsealRpc();
    }
    [ClientRpc]
    void UnsealRpc()
    {
        iUnlockableObject = objectToUnlock.GetComponent<IUnlockable>();
        iUnlockableObject.Unseal();
        unlockAnimation.Play();
    }
}
