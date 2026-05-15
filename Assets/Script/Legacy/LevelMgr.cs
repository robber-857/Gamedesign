using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//关卡管理
public class LevelMgr : SingletonMono<LevelMgr>
{
    //当前宝箱数量
    public int m_CurBaoXiangNums;

    //最大宝箱数量
    public int m_MaxBaoXiangNums;

    public Text m_BaoXiangNumT;

    public Text m_MaxBaoXiangNumT;

    public GameObject WinUI;

    //BOSS是否死亡
    public bool IsBossDeath;
    public bool BossDie;
    
    public GameObject NpcObj;

    public GameObject NpcObj1;

    //是否最后对话结束
    public bool IsLastDuiOver;
    //宝箱是否全部拾取完
    public bool IsAllGetBaoXiang;

    //传送们
    public GameObject ChuanSongMEN;
    private void Update()
    {
         if(m_BaoXiangNumT != null)
         {
            m_BaoXiangNumT.text = m_CurBaoXiangNums.ToString();
         }

        if(m_MaxBaoXiangNumT!=null)
        {
            m_MaxBaoXiangNumT.text = m_MaxBaoXiangNums.ToString();
        }

        if(IsBossDeath)
        {
            if(NpcObj!=null)
            {
                NpcObj1.SetActive(false);
                NpcObj.SetActive(true);
                IsBossDeath = false;
            } 
        }

        //判断游戏胜利
        if(IsLastDuiOver && IsAllGetBaoXiang && BossDie)
        {
            ChuanSongMEN.SetActive(true);
            //if (WinUI.activeSelf == false)
            //    WinUI.SetActive(true);
        }
    }

    //增加宝箱数量
    public void AddBaoXiangNums()
    {
        m_CurBaoXiangNums++;
        if(m_CurBaoXiangNums>=m_MaxBaoXiangNums)
        {
            m_CurBaoXiangNums = m_MaxBaoXiangNums;
            IsAllGetBaoXiang = true;
        }
    }

}
