using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Íæ¼ÒºìÐÇ¿ØÖÆ
public class PlayerRedXingC : MonoBehaviour
{
    public List<GameObject> m_HongXingList;
    public CharacterState m_State; 
    
    void Update()
    {
        for(int i=0;i< m_State.m_MaxHp;i++)
        {
            m_HongXingList[i].SetActive(false);
        }

        for (int i = 0; i < m_State.m_Hp; i++)
        {
            m_HongXingList[i].SetActive(true);
        }
    }
}
