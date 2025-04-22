using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IReadable
{
    public void EnterReadData(NetworkPlayerController owner);
}

[System.Serializable]
public struct UIReaderData
{
    public string titleKey;
    public string contentKey;
    public string passcode;
    public string tableEntry;
}

