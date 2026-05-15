using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDown : MonoBehaviour
{
    public GameObject item;
    
    public bool isTriggerDown = false;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player" &&!isTriggerDown)
        {
            isTriggerDown = true;
            item.gameObject.GetComponent<BoxCollider2D>().isTrigger = true;
            item.gameObject.GetComponent<Rigidbody2D>().gravityScale = 1;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
