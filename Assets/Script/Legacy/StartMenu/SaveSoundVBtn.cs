using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//保存音量脚本
public class SaveSoundVBtn : MonoBehaviour
{
    public AudioSource m_As;

    //是否第一次设置
    //0是
    //1不是
    private int m_IsFirstSet;

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            PlayerPrefs.SetFloat("M", m_As.volume);
            PlayerPrefs.SetInt("S", 1);
        });     
    }

  
}
