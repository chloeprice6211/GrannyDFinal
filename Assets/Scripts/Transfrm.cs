using UnityEngine;


namespace myTrs{
public class Transfrm : MonoBehaviour
{   
    
    public static void ResetPosition(Transform obj, Transform endPointTransform, Vector3 rotation){
         obj.SetParent(endPointTransform);
         obj.localPosition = Vector3.zero;
         obj.localRotation = Quaternion.Euler(rotation);
    }
    
}
}