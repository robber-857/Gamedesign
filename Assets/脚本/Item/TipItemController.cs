using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TipItemController : MonoBehaviour
{
    public GameObject tipPanel;


    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            tipPanel.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            tipPanel.SetActive(false);
        }
    }
    

    // Update is called once per frame
    void Update()
    {
        
    }
}
