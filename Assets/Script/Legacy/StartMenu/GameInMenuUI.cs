using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ÓÎÏ·ÄÚ²Ëµ¥
public class GameInMenuUI : MonoBehaviour
{
    public GameObject UI;

    private bool m_Ispress; 

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            m_Ispress = !m_Ispress;
            UI.SetActive(m_Ispress);
        }
    }
}
