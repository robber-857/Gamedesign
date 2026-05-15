using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Õ¨µ¯´¥·¢Æ÷
public class ZhaDanTrigger : MonoBehaviour
{
    public GameObject BaoZhaEffect;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "E")
        {
            collision.gameObject.GetComponent<CharacterState>().TakeDamge(gameObject, 0);
            GameObject.Destroy(gameObject);
            GameObject.Instantiate(BaoZhaEffect, transform.position, transform.rotation);
        }

        /*if (collision.gameObject.tag == "Box")
        {
            if (collision.gameObject.GetComponent<BoxController>() != null)
            {
                collision.gameObject.GetComponent<BoxController>().Smash();
            }

            if (collision.gameObject.GetComponent<TaskBoxController>() != null)
            {
                collision.gameObject.GetComponent<TaskBoxController>().Smash();
            }

            GameObject.Instantiate(BaoZhaEffect, transform.position, transform.rotation);
            GameObject.Destroy(gameObject);
        }*/
    }
}
