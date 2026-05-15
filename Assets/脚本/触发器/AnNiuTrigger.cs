using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnNiuTrigger : MonoBehaviour
{
    public Animator m_As;


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            m_As.enabled = true;
        }
    }

   
}
