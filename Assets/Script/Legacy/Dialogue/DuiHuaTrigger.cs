using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuiHuaTrigger : MonoBehaviour
{
    public bool IsDuiHuaOver;

    public GameObject showItem;

    public DialogPanel m_DialogPanel;

    public DialogData m_Data;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            gameObject.GetComponent<Collider2D>().enabled = false;
                m_DialogPanel.ShowDialog(m_Data, () =>
               {
                   if (showItem != null)
                   {
                       showItem.SetActive(true);
                   }

                   IsDuiHuaOver = true;
                     });
        }
    } 
    
    
}
