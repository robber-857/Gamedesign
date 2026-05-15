using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuiHuaEventTrigger : MonoBehaviour
{
    public GameObject NewObj;

    public BossController boss;
    void Update()
    {
        if(GetComponent<DuiHuaTrigger>().IsDuiHuaOver)
        { 
            if(name == "–°∫Ï√±1")
            {
                LevelMgr.GetInstance().IsLastDuiOver = true;
            }
            else
            {
                NewObj.SetActive(true);
                gameObject.SetActive(false);
                if (boss != null)
                {
                    ChangeMGM.GetInstance().ChangeBGMToBossFight();
                    boss.enabled = true;
                }
            }
                 
        }
    }
}
