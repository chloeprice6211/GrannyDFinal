using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInsertable
{
    public void Insert(UtilityItem itemToInsert, NetworkPlayerController owner);
}
