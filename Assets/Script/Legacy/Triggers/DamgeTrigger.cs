using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamgeTrigger : MonoBehaviour
{
    public bool IsOnce;

    private bool IsOne;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            if(IsOnce)
            {
                if (!IsOne)
                {
                    IsOne = true;
                    collision.gameObject.GetComponent<CharacterState>().TakeDamge(gameObject, 0);
                }
            }
            else
            {
                collision.gameObject.GetComponent<CharacterState>().TakeDamge(gameObject, 0);
            } 
        }
    }
}
