using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TangJiangMgr : MonoBehaviour
{
    public GameObject m_TangJiangPrefab;

    private float m_CurTime; 

    public float m_OverTime;

    
    void Update()
    {
        m_CurTime += Time.deltaTime;
        if(m_CurTime>= m_OverTime)
        {
            m_CurTime = 0; 
            GameObject.Instantiate(m_TangJiangPrefab,transform.position,transform.rotation);
        }
    }
}
