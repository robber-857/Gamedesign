using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookTextOp : MonoBehaviour
{
    public Animator anim;

    public GameObject item;
    // Start is called before the first frame update
    void Start()
    {
    }


    public void StartFade()
    {
        anim.SetTrigger("StartFade");
    }

    public void OverFade()
    {
        item.gameObject.SetActive(true);
        this.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
