using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpUpTrigger : MonoBehaviour
{ 
    public float m_Power;

    public GameObject target;

    public Animator Ar;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            target = collision.gameObject;
            Ar.SetTrigger("Tan");
        }
    }

    
}
