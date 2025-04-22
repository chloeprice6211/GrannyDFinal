using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGenerable
{
    public void ApplyGeneratedCode(string code);
    public void ShowGeneratedCode();
}
