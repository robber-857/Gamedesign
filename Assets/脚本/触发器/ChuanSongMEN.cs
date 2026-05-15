using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChuanSongMEN : MonoBehaviour
{
    public GameObject WinUI;
    void Start()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag=="Player")
        {
            WinUI.SetActive(true);
        }
    }
    
    void Update()
    {
        
    }
}
