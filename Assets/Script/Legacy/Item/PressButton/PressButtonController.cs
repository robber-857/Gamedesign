using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressButtonController : MonoBehaviour
{
    public bool canPress;

    public GameObject platForm;
    
    public Animator anim;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (canPress)
            {
                anim.SetTrigger("Press");
                platForm.gameObject.GetComponent<TwoPointVerticalMovement>().canMove = true;
            }
        }
    }

    public void CanPress()
    {
        canPress =  true;
    }

}
