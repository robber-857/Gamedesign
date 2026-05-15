using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathAreaTrigger : MonoBehaviour
{
    public GameObject item;
    

    private void OnTriggerEnter2D(Collider2D collision)
    { 
        if(collision.gameObject.tag == "Player")
        {
            LevelUIController ui = LevelUIController.FindOrCreate();

            if (ui != null)
            {
                ui.ShowFailure();
                return;
            }

            if (item != null)
            {
                item.gameObject.SetActive(true);
            }
        }
    }
}
