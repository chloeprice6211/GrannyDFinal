using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SwitchableButton : UIButton
{
    public bool isSelected;
    public List<SwitchableButton> linkedButtons;
    public LevelDifficultyType type;

    public override void OnClick()
    {

        switch (type)
        {
            case LevelDifficultyType.Easy:
                SetupPanel.LevelSettings = LevelSettings.Easy;
                //SetupPanel.Instance.CmdChangeDifficulty(LevelDifficulty.Easy);
                break;
            case LevelDifficultyType.Normal:
                SetupPanel.LevelSettings = LevelSettings.Normal;
                //SetupPanel.Instance.CmdChangeDifficulty(LevelDifficulty.Normal);
                break;
            case LevelDifficultyType.Nightmate:
                SetupPanel.LevelSettings = LevelSettings.Nightmare;
                //SetupPanel.Instance.CmdChangeDifficulty(LevelDifficulty.Nightmare);
                break;
        }

        isSelected = true;
        base.OnClick();

        carett.gameObject.SetActive(true);

        foreach(SwitchableButton button in linkedButtons)
        {
            button.Disselect();
        }

    }

    public override void OnMouseHover()
    {
        if (isSelected) return;

        base.OnMouseHover();
    }

    public override void OnMouseLeave()
    {
        if (isSelected) return;

        base.OnMouseLeave();
    }

    public void Disselect()
    {
        carett.gameObject.SetActive(false);
        isSelected = false;
        OnMouseLeave();

    }

}
