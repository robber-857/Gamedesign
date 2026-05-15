using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetTarget : MonoBehaviour
{
    public string m_TargetTag;

    public List<GameObject> m_Targets = new List<GameObject>(); 
  
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == m_TargetTag)
        {
            m_Targets.Add(collision.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == m_TargetTag)
        {
            m_Targets.Remove(collision.gameObject);
        }
    } 

}
