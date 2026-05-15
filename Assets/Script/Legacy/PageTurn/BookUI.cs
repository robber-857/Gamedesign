using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BookUI : MonoBehaviour
{
    //文字页面集合
    public List<PageMgr> m_PageMgrList;

    private int index;

    //下一页
    public Button m_NextPageBtn;

    //上一页
    public Button m_PrePageBtn;

    //当前文字页面
    public PageMgr m_CurPageMgr;

    //书动画
    public Animator m_BookAnimation;

    public GameObject StartGameBtn;


    void Start()
    {
        m_CurPageMgr = m_PageMgrList[0]; 
         
        //下一页
        m_NextPageBtn.onClick.AddListener(() =>
        {
            index++;
            if (index >2)
            {
                index = 2;
                
                return;
            }
                
            m_CurPageMgr.FadeOut();
        });

        //上一页
        m_PrePageBtn.onClick.AddListener(() =>
        {
            index--;
            if (index <0)
            {
                index = 0;
                return;
            }
            m_CurPageMgr.FadeOut1();
        });
    }

    //下翻页
    public void ChangePageNext() 
    {
        for(int i=0;i<m_PageMgrList.Count;i++)
        {
            m_PageMgrList[i].gameObject.SetActive(false);
        }

        
        //if(index>= m_PageMgrList.Count)
        //{
        //    index = 0; 
        //}

        m_PageMgrList[index].gameObject.SetActive(true);
        m_CurPageMgr = m_PageMgrList[index];
    }

    //上翻页
    public void ChangePagePre()
    {
        for (int i = 0; i < m_PageMgrList.Count; i++)
        {
            m_PageMgrList[i].gameObject.SetActive(false);
        }

         
        //if (index<0)
        //{
        //    index =  0; 
        //}

        m_PageMgrList[index].gameObject.SetActive(true);
        m_CurPageMgr = m_PageMgrList[index];
    }

    private void Update()
    {
        if (index == 2)
            StartGameBtn.SetActive(true);
        else
            StartGameBtn.SetActive(false);

    }
}
