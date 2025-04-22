using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public interface IUnlockable
{
    public void Unseal();
    public bool Check();
}
