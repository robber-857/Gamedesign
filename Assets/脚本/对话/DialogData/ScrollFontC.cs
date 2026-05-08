using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollFontC : MonoBehaviour
{
    public Text m_StrT;

    public string m_Str;

    private int m_Index;

    private float m_CurTime;

    //¼ä¸ôÊ±¼ä
    public float m_PerTime;

    public void StartGo(string _str)
    {
        
        m_Index = 0;
        m_Str = _str; 
        m_StrT.text = "" + m_Str[m_Index];
        m_CurTime = 0;
        m_Index++;
    }
 
    void Start()
    {
                
    }
 
    void Update()
    {
        m_CurTime += Time.deltaTime;
        if (m_CurTime >= m_PerTime)
        {
          
            if (m_Index >= m_Str.Length)
                return;
            m_CurTime = 0;
            m_StrT.text += "" + m_Str[m_Index];
            m_Index++;
        }
    }
}
