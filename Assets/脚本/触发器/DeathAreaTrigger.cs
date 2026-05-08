using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class DeathAreaTrigger : MonoBehaviour
{
    public GameObject item;
    

    private void OnTriggerEnter2D(Collider2D collision)
    { 
        if(collision.gameObject.tag == "Player")
        {
            item.gameObject.SetActive(true);
        }
    }
}
