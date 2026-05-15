using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoGuTrigger : MonoBehaviour
{
    public int Hp;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            GameObject.Destroy(gameObject);
            collision.gameObject.GetComponent<CharacterState>().AddHp(Hp);
        }
    }
   
}
