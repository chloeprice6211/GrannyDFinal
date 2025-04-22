using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum NoteObjectiveType
{
    Router,
    Email,
    Phone
}

public class GeneratedNote : Note, IReceivePassword
{
    [SerializeField] TextMeshProUGUI generatedCodeText;

    public string code;
    public int passcodeLenght;
    public bool isNumeric;
    public NoteObjectiveType objectiveType;

    public void DisplayPassword(string code)
    {
        Debug.Log(code);
        this.code = code;
        generatedCodeText.text = code;
    }

    public override void EnterReadData(NetworkPlayerController owner)
    {
        readerData.passcode = generatedCodeText.text;
        base.EnterReadData(owner);
    }
}
