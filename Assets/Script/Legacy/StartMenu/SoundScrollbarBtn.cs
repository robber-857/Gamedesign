using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//音量条 
public class SoundScrollbarBtn : MonoBehaviour
{
    public AudioSource m_As;

    private void OnEnable()
    { 
        int isfirst = PlayerPrefs.GetInt("S");
        if (isfirst == 0)
        {
            m_As.volume = 1;
            GetComponent<Scrollbar>().value = 1;
        }
        else
        { 
            float v = PlayerPrefs.GetFloat("M"); 
            GetComponent<Scrollbar>().value = v;
        }
    }
    void Update()
    {
        //音量更新
        m_As.volume = GetComponent<Scrollbar>().value;
    }
}
