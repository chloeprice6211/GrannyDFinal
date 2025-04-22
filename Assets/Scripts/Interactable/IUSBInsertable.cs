using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUSBInsertable
{
    public void InsertUSB(FlashDriver usb);
    public void ShowHiddenFile();
}
