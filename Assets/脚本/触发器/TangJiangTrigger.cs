using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ÌÇ½¬´¥·¢Æ÷
public class TangJiangTrigger : MonoBehaviour
{
      
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<CharacterState>().TakeDamge(gameObject, 0);
            GameObject.Destroy(gameObject);
        }
    }
}
