using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskBoxManger : MonoBehaviour
{
    public static TaskBoxManger instance;

    public int currentSmashBox = 0;
    public int totalSmashBox = 3;

    public GameObject jumpItem;
    private void Awake()
    {
        instance = this;
    }

    public void AddSmashBox()
    {
        currentSmashBox++;
        CheckOver();
    }

    public void CheckOver()
    {
        if (currentSmashBox == totalSmashBox)
        {
            jumpItem.gameObject.SetActive(true);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
