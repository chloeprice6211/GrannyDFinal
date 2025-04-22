using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IBMKeypad : NewKeypad
{
    public NewMonitor monitor;

    public override void AddDigit(char digit)
    {
        if (!monitor.isFunctional)
        {
            UIManager.Instance.Message("turnComputer", "turnComputer_A");
            return;
        }

        base.AddDigit(digit);
    }

    public override void Verify()
    {
        if (!monitor.isFunctional)
        {
            UIManager.Instance.Message("turnComputer", "turnComputer_A");
            return;
        }

        base.Verify();
    }
}
