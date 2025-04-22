using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tip : MonoBehaviour
{
    public string message;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            Invoke("Message", 2f);
        }
    }

    private void Message()
    {
        UIManager.Instance.Message(message, "powerless_A");

        Destroy(gameObject);
    }
}
