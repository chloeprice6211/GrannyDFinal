using UnityEngine;

public class SwatOperator : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Animation motion;
    public Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
