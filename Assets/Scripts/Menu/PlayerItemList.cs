using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class PlayerItemList : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI playerNameText;
    [SerializeField] TextMeshProUGUI playerStatusText;

    private string playerName;
    private string readyStatus;

    public string PlayerName
    {
        set
        {
            playerNameText.text = value;
        }
    }
    public string PlayerStatus
    {
        set
        {
            playerStatusText.text = value;
        }
    }
    
}
