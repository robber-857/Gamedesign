using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PageMgr : MonoBehaviour
{
    public TextFadeEffect m_Page1;
    public TextFadeEffect m_Page2;

    public BookUI bookui;
    void Start()
    {
        
        FadeIn();
        //m_Page1.OnFadeOutComplete += () =>
        //{
        //    Debug.Log("µ­³ö³É¹¦");
        //    bookui.m_BookAnimation.SetTrigger("FanYe");
        //};

        
    }

    private void OnEnable()
    {
        FadeIn();
    }
    public void FadeOut()
    { 
        m_Page1.OnFadeOutComplete = OnfadeOutComplelte1; 

        m_Page1.StartFadeOut();
        m_Page2.StartFadeOut();
    }

    public void OnfadeOutComplelte1()
    {
        bookui.m_BookAnimation.SetTrigger("FanYe");
    }
    public void FadeOut1()
    { 
        m_Page1.OnFadeOutComplete  = OnfadeOutComplelte2; 

        m_Page1.StartFadeOut();
        m_Page2.StartFadeOut();
    }
    public void OnfadeOutComplelte2()
    {
        bookui.m_BookAnimation.SetTrigger("FanYe1");
    }

    public void FadeIn()
    {
        m_Page1.StartFadeIn();
        m_Page2.StartFadeIn();
    } 
}
