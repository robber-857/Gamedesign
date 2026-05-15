using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stunt : MonoBehaviour
{
    public bool isStunt = false;

    public Rigidbody2D rb;

    public float originalPos;

    private void Awake()
    {
        rb = this.GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        originalPos = rb.gravityScale;
        
    }


    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.U))
        {
            if (!isStunt)
            {
                isStunt = true;

                rb.gravityScale = -originalPos;
                this.transform.localScale = new Vector3(transform.localScale.x, -transform.localScale.y, transform.localScale.z);
                this.gameObject.GetComponent<PlayerController>().jumpForce =
                    -this.gameObject.GetComponent<PlayerController>().jumpForce;
                return;
            }

            if (isStunt)
            {
                isStunt = false;
                this.transform.localScale = new Vector3(transform.localScale.x, -transform.localScale.y, transform.localScale.z);
                this.gameObject.GetComponent<PlayerController>().jumpForce =
                    -this.gameObject.GetComponent<PlayerController>().jumpForce;
                rb.gravityScale = originalPos;
                return;
            }
        }
    }
}
