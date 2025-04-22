using UnityEngine;

public class PowerCable : MonoBehaviour
{
    public Rigidbody rb;

    [QFSW.QC.Command ("uncut")]
    public void OnCut(){
        rb.isKinematic = false;
    }
}
