using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoes : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            //IceGroundAddForce.instance.GetShoes();
            Destroy(this.gameObject);
        }
    }
}
