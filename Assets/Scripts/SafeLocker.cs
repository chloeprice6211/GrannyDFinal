using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeLocker : MonoBehaviour
{
    public List<int> combinationArray = new()
    {
        10,
        20,
        30
    };

    public List<int> currentCombinationArray;

    public bool isUnlocked;

    public void PassCombination(int combination)
    {
        currentCombinationArray.Add(combination);

        if (CheckCombination())
        {
            isUnlocked = true;
            Debug.Log("unlocked");
        }
       
    }

    bool CheckCombination()
    {
        for (int a = 0; a < 3; a++)
        {
            if (currentCombinationArray[a] != combinationArray[a])
            {
                return false;
            }
        }

        return true;
    }
}
