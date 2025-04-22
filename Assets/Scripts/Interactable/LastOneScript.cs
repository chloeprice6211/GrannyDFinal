using UnityEngine;

public class LastOneScript : MonoBehaviour, ITriggered
{   
    public SwatOperator swat;
    public Animation swatCarDoors;
    public void OnTrigger()
    {

        swat.animator.SetBool("hasTriggered", true);
        swatCarDoors.Play();

    }
}
