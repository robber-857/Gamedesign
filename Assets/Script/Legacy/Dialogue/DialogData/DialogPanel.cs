using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events; 

public class DialogPanel : MonoBehaviour
{ 
    //对话数据
    public DialogData data;

    public KeyCode m_KeyCode;

    //对话结束后得回调
    public UnityAction m_Fuc; 

    //对话索引
    private int dialogIndex = 0; 

    //头像 
    public Image Headimage;

    //名字
    public Text nameTxt;
    
    //内容
    public Text contentTxt;

    //背景图
    public Image m_Back;

    public ScrollFontC m_ScrollFontC;

    public AudioSource m_AS;

    private void Start()
    {
        
    }

    void Update()
    {
        if(Input.GetKeyDown(m_KeyCode))
        {
            Next();
        }
    }  

    //显示对话
    public void ShowDialog(DialogData d, UnityAction _func = null)
    {
        Debug.Log("2222");
        data = d;
        dialogIndex =0;
        m_Fuc = _func; 
        UpdateDialog(); 
        gameObject.SetActive(true);
        m_ScrollFontC.StartGo(data.dialogData[dialogIndex].content);
        m_AS.clip = data.dialogData[dialogIndex].audioclip;
        m_AS.Play();

    }

    //对话更新
    public void UpdateDialog()
    {
        if (dialogIndex >= data.dialogData.Count)
        {
            gameObject.SetActive(false);  
            return;
        }

        if (data.dialogData[dialogIndex].sprite != null)
        {
            Headimage.gameObject.SetActive(true);
            Headimage.sprite = data.dialogData[dialogIndex].sprite;
        }
        else
        {
            Headimage.gameObject.SetActive(false);
        }

        nameTxt.text = data.dialogData[dialogIndex].name; 
    }

    
    //下一句
    public void Next()
    { 
        dialogIndex++;
        
        //当对话完成后做什么 
        if(dialogIndex==data.dialogData.Count)
        {
            gameObject.SetActive(false); 

            //执行外部得回调函数，完成后做什么
            if(m_Fuc!=null)
            {
                m_Fuc.Invoke();
            } 
            return;
        }

        UpdateDialog();

        //设置滚动字内容
        m_ScrollFontC.StartGo(data.dialogData[dialogIndex].content);

        m_AS.clip = data.dialogData[dialogIndex].audioclip;
        m_AS.Play();
    } 
}
