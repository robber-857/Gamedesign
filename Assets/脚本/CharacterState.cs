using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//属性
public class CharacterState : MonoBehaviour
{
    //伤害数字
    public GameObject m_DamgeTextRes;
    public Transform m_DamgeRoot;

    //被击打特效
    public GameObject m_HitEffect;

    //血条
    public Image m_HpBar;
    public GameObject m_HpBarRoot;

    public int m_Id; 
    //名字
    public string m_RoleName;

    //等级
    public int m_Level;

    //血量
    public int m_Hp; 
    public int m_MaxHp;

    //魔法值
    public int m_Mp;
    public int m_MaxMp;

    //当前体力
    public int m_CurTili; 
    //最大体力
    public int m_MaxTili;

    //基础经验
    public int m_BaseExp;

    //当前经验
    public int m_CurExp;

    //最大经验
    public int m_MaxExp;

     //护盾值
    public int m_SheldValue; 
    
    //基础物理伤害
    public int phyiscDamgeBase;
    //物理防御
    public int PhysicDefine;
    
    //基础魔法伤害
    public int MagicDamgeBase;
    //魔法防御 
    public int MagicDefine;

    //暴击几率(0-1)
    public float m_BaoJiRate;

    //攻击速度
    public float m_AttackSpeed;

    //是否存在护盾
    public bool m_IsHaveSheld  = false;

    //金币
    public int m_Gold;
     

    //升级特效
    public GameObject m_LevelUpEffect;


    public GameObject GamevOverUI;

    //是否有掉落
    public bool IsDrop;

    public void TakeDamge(GameObject _atter,int _DamgeType)
    {
        Debug.Log("收到攻击");
        if (m_Hp <= 0)
            return;

        //攻击者属性引用
        CharacterState attackstate =  _atter.GetComponent<CharacterState>();
          //核心伤害
        int coredamge = 0;
        if(_DamgeType == 0) //物理伤害 
            coredamge = Mathf.Max(0,attackstate.phyiscDamgeBase - PhysicDefine); 
        else if(_DamgeType == 1)   //魔法伤害 
            coredamge =  Mathf.Max(0,attackstate.MagicDamgeBase - MagicDefine);   
         
        //显示伤害数字
        if(m_DamgeTextRes!=null)
        {
            GameObject damgeui =  GameObject.Instantiate(m_DamgeTextRes, transform.position, transform.rotation);
            damgeui.GetComponent<Text>().text = coredamge.ToString();
            damgeui.transform.SetParent(m_DamgeRoot, false);
        }

        //显示击打特效
        if(m_HitEffect!=null)
        {
            GameObject.Instantiate(m_HitEffect, transform.position, transform.rotation);

        }
        //获取攻击者伤害
        m_Hp -= coredamge;  
           
        if (m_Hp<=0)
        {
            if(tag == "E")
            {
                if (GetComponent<Animator>() != null)
                    GetComponent<Animator>().SetTrigger("Die");
              

                if (name == "BOSS")
                {
                    LevelMgr.GetInstance().IsBossDeath = true;
                    LevelMgr.GetInstance().BossDie = true;
                    GetComponent<Collider2D>().isTrigger = true;
                    GetComponent<Rigidbody2D>().gravityScale = 0;
                }  
                if (name == "狼")
                {
                    GameObject.Destroy(gameObject, 0.5f);
                    transform.position -= new Vector3(0, 0.6f);
                    GetComponent<Collider2D>().isTrigger = true;
                    GetComponent<Rigidbody2D>().gravityScale = 0;
                } 
                else
                {
                    GameObject.Destroy(gameObject, 0.5f);
                    //GetComponent<Collider2D>().isTrigger = true;
                    //GetComponent<Rigidbody2D>().gravityScale = 0;
                }
                if (name == "BOSS")
                {
                    transform.position -= new Vector3(0, 1.2f);
                    GameObject.Destroy(gameObject, 1.2f);
                    GetComponent<Collider2D>().isTrigger = true;
                    GetComponent<Rigidbody2D>().gravityScale = 0;
                } 
                else
                {
                    if(name == "箱子")
                    {
                        GetComponent<Animator>().SetBool("Po", true);
                        GameObject.Destroy(gameObject, 1.5f);
                        

                    }
                    else
                    {
                        GameObject.Destroy(gameObject, 0.5f);
                    } 
                }

                //如果有掉落
                if(IsDrop)
                {
                    GetComponent<DropItem>().Drop();
                }
            }
            
            else if(tag == "Player")
            {
                GetComponent<Animator>().SetTrigger("Die");
                GameObject.Destroy(gameObject, 2);
                GamevOverUI.SetActive(true);
            } 

            
           
        } 

        //被攻击后显示血条一段时间
        if(tag == "Enemy")
        {
            if(m_HpBarRoot.activeSelf == false)
            {
                m_HpBarRoot.SetActive(true);
            } 
        }

        //被攻击动画
        if (GetComponent<Animator>()!=null)
            GetComponent<Animator>().SetTrigger("Hit");
    } 

    //加血
    public void AddHp(int _hp)
    {
        m_Hp += _hp;
        if(m_Hp >= m_MaxHp)
        {
            m_Hp = m_MaxHp;
        }
    }   

    

    //升级
    public void LevelUp(CharacterState _Attater)
    {
        _Attater.m_CurExp += m_BaseExp;

        //当经验达到
        if(_Attater.m_CurExp>=_Attater.m_MaxExp)
        {
            //播放特效
            _Attater.m_LevelUpEffect.SetActive(true); 
            _Attater.m_CurExp  = 0;
            _Attater.m_Level++;
            //注意这里的升级公示可以修改
            _Attater.m_MaxExp += (_Attater.m_MaxExp + (int)(_Attater.m_Level * 2));
            //基本数据提升-升级之后增加多少属性都可以自己修改
            _Attater.m_MaxHp +=100; 
            _Attater.m_Hp = _Attater.m_MaxHp;
            _Attater.m_MaxMp +=100;
            _Attater.m_Mp = _Attater.m_MaxMp;
            _Attater.PhysicDefine += 10;
            _Attater.MagicDefine +=10;
        } 
    } 

    private void Update()
    {
        //血条更新
        if(m_HpBar!=null)
        {
            m_HpBar.fillAmount = (float)m_Hp / m_MaxHp;
        }
    }

    //消耗魔法
    public bool CostMaigic(int _value) 
    {
        if (m_Mp >= _value)
        {
            m_Mp -= _value;
            if (m_Mp <= 0)
                m_Mp = 0;
            return true;
        }

        return false;
    }
}
