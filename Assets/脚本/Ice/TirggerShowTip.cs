using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TirggerShowTip : MonoBehaviour
{
    public GameObject tipsItemPanl;

    
    public Text infoText;
    public string tipInfo;
    
    public bool isShow = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player" && !isShow)
        {
            tipsItemPanl.SetActive(true);
            infoText.text = tipInfo;
            isShow = true;
            Invoke("Hide",1f);
        }
    }

    public void Hide()
    {
        tipsItemPanl.SetActive(false);
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
