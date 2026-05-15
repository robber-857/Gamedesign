using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GoToSceneBtn : MonoBehaviour
{
    public string m_SceneName; 
   
    void Start()
    {
        if (GetComponent<Button>() == null) return;
        GetComponent<Button>().onClick.AddListener(() =>
        {
            SceneManager.LoadScene(m_SceneName);
        });  
    }

    public void LoadScene()
    {
        SceneManager.LoadScene(m_SceneName);
    }
}
