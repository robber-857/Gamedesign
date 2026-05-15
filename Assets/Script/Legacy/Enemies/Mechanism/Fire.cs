using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour
{
    public bool isDownItem = false;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            other.gameObject.GetComponent<CharacterState>().TakeDamge(this.gameObject, 0);
            if (isDownItem)
            {
                
                Destroy(this.gameObject);
            }
        }
    }
}
