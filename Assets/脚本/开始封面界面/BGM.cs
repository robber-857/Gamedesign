using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//背景音乐
public class BGM : MonoBehaviour
{ 
    void Start()
    {
        //从保存的数据中读取音量
        //判断是不是第一次设置，如果是音量默认是1
        int isfirst = PlayerPrefs.GetInt("S");
        if(isfirst == 0)
        {
            Debug.Log("第一次设置音量");
            GetComponent<AudioSource>().volume = 1;
        }
        else
        {
            Debug.Log("已经设置过音量");
            float v = PlayerPrefs.GetFloat("M");
            GetComponent<AudioSource>().volume = v;
        }
    } 
}
