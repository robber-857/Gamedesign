using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public CharacterState m_Onwer;

    //目标标签
    public string m_TargetTag;

    
    //击中后特效
    public GameObject m_HitEffect;

    public float m_MoveSpeed;

    //方向
    public float m_Dir;

    //初始化数据
    public void InitData(CharacterState _owner,  float _speed, float _dir)
    {
        m_Onwer = _owner; 
        m_MoveSpeed = _speed;
        m_Dir = _dir;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == m_TargetTag)
        {
            //伤害处理
            CharacterState target = collision.gameObject.GetComponent<CharacterState>();
            if(target!=null)
            {
                target.TakeDamge(m_Onwer.gameObject, 0);
            }
            //播放击中效果
            if (m_HitEffect != null)
                m_HitEffect.SetActive(true);
            GameObject.Destroy(gameObject);
        }
    }

    private void Update()
    {
        if(m_Dir>=0)
        {
            transform.Translate(Vector2.right * Time.deltaTime * m_MoveSpeed);
            transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            transform.Translate(-Vector2.right * Time.deltaTime * m_MoveSpeed);
            transform.localScale = new Vector3(-1, 1, 1);
        }
        
    }
}
