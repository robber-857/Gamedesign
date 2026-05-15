using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StuntTrigger : MonoBehaviour
{
    public bool use;

    public Stunt playerStunt;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (use)
            {
                playerStunt.enabled = true;
            }
            else
            {
                playerStunt .enabled = false;
            }
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
