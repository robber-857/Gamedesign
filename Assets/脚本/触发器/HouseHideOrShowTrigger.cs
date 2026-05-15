using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseHideOrShowTrigger : MonoBehaviour
{
    public Animator m_HousAr;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            m_HousAr.SetBool("Show", true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            m_HousAr.SetBool("Show", false);
        }
    }
}
