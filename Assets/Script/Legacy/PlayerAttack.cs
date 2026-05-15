using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private float m_CurTime;

    public bool m_IsAttack;

    public float m_AttackTime;

    public Transform m_AttackPos;

    public GameObject m_PrefabsArow;

    public GetTarget m_GetTarget;

    public AudioSource m_AttackAs;

    public GameObject ZhaDanObj;

    void Update()
    {
        //¹¥»÷
        if(Input.GetKeyDown(KeyCode.J))
        {
            if(!m_IsAttack)
            {
                GetComponent<Animator>().SetTrigger("Attack");
                m_IsAttack = true;
                m_AttackAs.Play();
            }
          
        }

        //Í¶Õ¨µ¯
        if(Input.GetKeyDown(KeyCode.K))
        {
            if (!m_IsAttack)
            {
               // GetComponent<Animator>().SetTrigger("Attack");
                m_IsAttack = true;
                GameObject zhadan= GameObject.Instantiate(ZhaDanObj, m_AttackPos.position, m_AttackPos.rotation);

                 if(transform.localScale.x >0)
                {
                    zhadan.GetComponent<Rigidbody2D>().AddForce(Vector2.right * 5,ForceMode2D.Impulse);
                    zhadan.GetComponent<Rigidbody2D>().AddForce(Vector2.up * 12, ForceMode2D.Impulse);
                }
                 else
                {
                    zhadan.GetComponent<Rigidbody2D>().AddForce(-Vector2.right * 5, ForceMode2D.Impulse);
                    zhadan.GetComponent<Rigidbody2D>().AddForce(Vector2.up * 12, ForceMode2D.Impulse);
                }
                 
            }
        }

        if(m_IsAttack)
        {
            m_CurTime += Time.deltaTime;
            if(m_CurTime>= m_AttackTime)
            {
                m_CurTime = 0;
                m_IsAttack = false;
            }
        }
    }

    public void Event_ArowAttack()
    {
        //GameObject item =   GameObject.Instantiate(m_PrefabsArow, m_AttackPos.position, m_AttackPos.rotation);
        //item.GetComponent<Arrow>().InitData(gameObject.GetComponent<CharacterState>(), 16, GetComponent<PlayerController>().GetDirection());
    }

    public void Event_JinZhanAttack()
    {
        if(m_GetTarget!=null)
        {
            for(int i=0;i<m_GetTarget.m_Targets.Count;i++)
            {
                if(m_GetTarget.m_Targets[i]!=null)
                {
                    m_GetTarget.m_Targets[i].GetComponent<CharacterState>().TakeDamge(gameObject, 0);
                }
            }
        }
    }
}
