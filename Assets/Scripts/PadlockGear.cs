using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PadlockGear : HighlightElement
{
    [SerializeField] CombinationLock locker;
    public int gearIndex;
    public int currentChar;

    public void ChangeChar()
    {
        currentChar++;
        if (currentChar == 10) currentChar = 0;

        transform.Rotate(new Vector3(0, 36, 0));

        ApplyGearChar();
    }

    void ApplyGearChar()
    {
        locker.Verify(gearIndex);
    }
}
