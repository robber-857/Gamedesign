using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//µôÂä´¥·¢Æ÷
public class DiaoLuoObjTrigger : MonoBehaviour
{
    public Rigidbody2D m_Rig;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            m_Rig.gravityScale = 1;
        }
    }
     
    
}
